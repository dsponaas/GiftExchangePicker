using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GiftExchangePicker
{
    class Program
    {
        static void Main( string[] args )
        {
            List<GiftPerson> giftPersonList = new List<GiftPerson>
            {
                new GiftPerson
                {
                    Name = "Person1",
                    Email = "Person1",
                    NameExclusionList = new List<string>{ "Person1", "Person2" }
                },
                new GiftPerson
                {
                    Name = "Person2",
                    Email = "Person2",
                    NameExclusionList = new List<string>{ "Person1", "Person2" }
                },
                new GiftPerson
                {
                    Name = "Person3",
                    Email = "Person3",
                    NameExclusionList = new List<string>{ "Person3", "Person4" }
                },
                new GiftPerson
                {
                    Name = "Person4",
                    Email = "Person4",
                    NameExclusionList = new List<string>{ "Person3", "Person4" }
                },
                new GiftPerson
                {
                    Name = "Person5",
                    Email = "Person5",
                    NameExclusionList = new List<string>{ "Person5", "Person6" }
                },
                new GiftPerson
                {
                    Name = "Person6",
                    Email = "Person6",
                    NameExclusionList = new List<string>{ "Person5", "Person6" }
                }
            };
            while( true )
            {
                XmasGiftPicker( giftPersonList, "PERSON_WHO_RECIEVES_THE_MASTER_LIST" );
                Console.ReadLine();
            }
        }

        public class GiftPerson
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public List<string> NameExclusionList { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }

        public static void XmasGiftPicker( List<GiftPerson> personList, string masterListRecipient )
        {
            List<GiftPerson> listToSelectFrom = new List<GiftPerson>();
            List<Tuple<GiftPerson, GiftPerson>> selectionList = new List<Tuple<GiftPerson, GiftPerson>>();
            bool finished = false;
            Random rand = new Random();
            int iterationCounter = 1;
            while( !finished )
            {
                selectionList.Clear();
                foreach( var cur in personList )
                {
                    listToSelectFrom.Add( new GiftPerson
                    {
                        Name = cur.Name,
                        Email = cur.Email,
                        NameExclusionList = cur.NameExclusionList
                    } );
                }
                finished = true;
                foreach( var curOuter in personList )
                {
                    int selection = rand.Next( listToSelectFrom.Count );
                    Tuple<GiftPerson, GiftPerson> toAdd = Tuple.Create( curOuter, listToSelectFrom[ selection ] );
                    selectionList.Add( toAdd );
                    listToSelectFrom.RemoveAt( selection );
                }
                foreach( var curSelection in selectionList )
                {
                    if( curSelection.Item1.NameExclusionList.Contains( curSelection.Item2.Name ) )
                    {
                        finished = false;
                    }
                }
                Console.WriteLine( $"iteration: {iterationCounter}" );
                ++iterationCounter;
            }
            Console.WriteLine( "***SELECTION MADE***" );
            StringBuilder sb = new StringBuilder();
            using( var smtp = new SmtpClient() )
            {
                foreach( var cur in selectionList )
                {
                    MailMessage msg = new MailMessage();
                    sb.AppendLine( $"{cur.Item1.Name} picked {cur.Item2.Name}" );
                    //Console.WriteLine( $"{cur.Item1.Name} picked {cur.Item2.Name}" );
                    msg.To.Add( cur.Item1.Email );
                    msg.Subject = "Xmas Gift Selection";
                    msg.IsBodyHtml = false;
                    msg.Body = $"{cur.Item1.Name} picked {cur.Item2.Name}";
                    smtp.Send( msg );
                }
                MailMessage masterListMsg = new MailMessage();
                masterListMsg.To.Add( masterListRecipient );
                masterListMsg.Subject = "Xmas Gift Selection";
                masterListMsg.IsBodyHtml = false;
                masterListMsg.Body = sb.ToString();
                smtp.Send( masterListMsg );
            }
        }
    }
}
