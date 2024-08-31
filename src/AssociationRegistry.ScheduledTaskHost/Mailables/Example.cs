using Coravel.Mailer.Mail;

namespace AssociationRegistry.ScheduledTaskHost.Mailables
{
    public class Example : Mailable<string>
    {
        public Example()
        {
            // Inject a model if using this.View()
        }

        public override void Build()
        {
            this.To("coravel@is.awesome")
                .From("from@test.com")
                .View("~/Views/Mail/Example.cshtml", null);
        }
    }
}
