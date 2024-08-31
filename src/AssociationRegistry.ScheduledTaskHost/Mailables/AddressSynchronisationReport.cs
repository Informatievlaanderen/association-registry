using Coravel.Mailer.Mail;

namespace AssociationRegistry.ScheduledTaskHost.Mailables
{
    public class AddressSynchronisationReport : Mailable<string>
    {
        public AddressSynchronisationReport()
        {
            // Inject a model if using this.View()
        }

        public override void Build()
        {
            this.To("coravel@is.awesome")
                .From("from@test.com")
                .View("~/Views/Mail/AddressSynchronisationReport.cshtml", null);
        }
    }
}
