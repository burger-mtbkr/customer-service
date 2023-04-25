## Future improvements to consider💡

**Design:**

- This api includes functionality for:

  - Customers,
  - Leads,
  - Authorization,
  - Users
  - Signup

  - For a microservice architecture I would split these up into their own apis that can be deployed as individual components but thats out of scope for this assessment.

**Development experience**

- Setup local reverse proxy to make it easier for the SPA and API to both run on localhost - I have added some information to do this in the SPA documentation.

**Validation improvements**

- For validations I think it would be preferred to have a validation action filter for both Leads and Customers.
- We should also consider what makes a customer or a lead a duplicate. For example if the customer is added and another exists with the same email, First and and last name do we treat them as a customer that already exists?
- The same will apply to leads, although for leads the rules may be different in that the same customer could have many leads over time.

**Permissions:**

- Permission management would be implemented to allow who can access update customers and leads.

**Testing:**

- I have added unit test coverage for the main services but not for any of the infrastructure of setup code.
- An good improvement would be to extend the test coverage to cover all parts of the system.
- Integration testing would also be a big advantage. Using something like [Wiremock](https://github.com/WireMock-Net/WireMock.Net) will enable us to test that our contracts stay in tact with that the SPA will expect. I recommend a contract first approach.

**Security**

- Currently there are some key and potential sensitive data in the `appsettings.json` file. These should be moved to something like AWS parameter store. We can then retrieve these settings in the pipeline at the time of deployment and populate them either into the `appsettings.json`
- Encryption can be turned on in the `JsonFlatFileDataStore` datastore or we can consider upgrading to something like SQL Server
- Export of customer and leads will be useful.
- Enable allowed domains (cors).
- API should be hosted over HTTPS.

**Observability:**

- Consider what observability we want and logging.
- Tools like new relic and Sumo logic would be very valuable for and can be used to setup alerts for any potential issues.
- For now I have added serilog logs that will write to the console and to a text file
