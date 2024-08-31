using Coravel.Mailer.Mail;

namespace AssociationRegistry.ScheduledTaskHost.Mailables
{
    public class AddressSynchronisationReport : Mailable<AddressSynchronisationReportModel>
    {
        private readonly AddressSynchronisationReportModel _model;
        public AddressSynchronisationReport(AddressSynchronisationReportModel model) => _model = model;

        public override void Build() => this
                                       .To("orca-devs@vlaanderen.be")
                                       .From("digitaal.vlaanderen@vlaanderen.be")
                                       .View("~/Views/Mail/AddressSynchronisationReport.cshtml", _model);
    }

    public class AddressSynchronisationReportModel
    {
    }
}
