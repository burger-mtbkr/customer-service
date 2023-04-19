## Future improvements to consider💡

**Feature improvements:**

- For this assessment I have not included server side searching and filtering but it would be a good addition to add if we extend this it include pagination or want to limit the results being returned.  I will handle that requirement in the SPA.

**Permissions:**

- Permission management would be implemented to allow who can access update customers and leads.

**Testing:**

- I have added unit test coverage for the main services but not for any of the infrastructure of setup code.
- An good improvement would be to extend the test coverage to cover all parts of the system.
- Integration testing would also be a big advantage.  Using something like [Wiremock](https://github.com/WireMock-Net/WireMock.Net) will enable us to test that our contracts stay in tact with that the SPA will expect. I recommend a contract first approach.

**Security**

- Currently there are some key and potential sensitive data in the `appsettings.json` file.  These should be moved to something like AWS parameter store.  We can then retrieve these settings in the pipeline at the time of deployment and populate them either into the `appsettings.json`
- Encryption can be turned on in the `JsonFlatFileDataStore` datastore or we can consider upgrading to something like SQL Server
- Export of customer and leads will be useful.

**Observability:**

- Consider what observability we want and logging.
- Tools like new relic and Sumo logic would be very valuable for and can be used to setup alerts for any potential issues.