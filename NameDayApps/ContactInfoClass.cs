using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NameDayApps
{
    public class ContactInfoClass
    {
        //this field is read-only because it have ho setter, and have only the getter
        public Contact Contact { get; } //The Contact is a built-in contact manager

        public ContactInfoClass(Contact contact)
        {
            Contact = contact;
        } 
        public ContactInfoClass(string firstName, string lastName)
        {
            Contact = new Contact()
            {
                FirstName = firstName,
                LastName = lastName
            }; 
        }

        //getting the initials Characters of First and Last name like Junaid Ahmed => JA
        public string Initials => GetFirstCharacter(Contact.FirstName) +
            GetFirstCharacter(Contact.LastName);

        //getting the first characters of word
        public string GetFirstCharacter(string s) => 
            string.IsNullOrEmpty(s) ? "" : s.Substring(0, 1);


        //Visible Email icon if the email property has at least one email in contact
        //otherwise it will not be visible ...//UWP control an item through visibility Enum instead of just to Boolean value
        public Visibility EmailVisibility =>
            DesignMode.DesignModeEnabled ||
            Contact.Emails.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        //it is difficult to set the property of image
        //The Picture property, property getter can not be async
        public ImageBrush Picture
        {
            get
            {
                if (Contact.SmallDisplayPicture == null)
                    return null;

                var image = new BitmapImage();

                image.SetSource(Contact.SmallDisplayPicture.OpenReadAsync()
                    .GetAwaiter().GetResult());

                return new ImageBrush { ImageSource = image };
            }
        }
    }
}
