Feature: Registreer Feitelijke Vereniging

  Scenario: All Fields
    Given Feitelijke vereniging werd geregistreerd met alle velden
    When Registreer feitelijke vereniging endpoint has been called
    Then It saves the events
    And It returns an accepted response
    And It returns a location header
    And It returns a sequence header
    And It should have place a message on sqs for address match

  Scenario: All Fields And Postal Information

