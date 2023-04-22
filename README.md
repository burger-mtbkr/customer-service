## Spidertracks Customer API

This is a .Net 7 API with a swagger ui utilising a [JsonFlatFileDataStore](https://github.com/ttu/json-flatfile-datastore). I have added some [assumptions](/docs/ASSUMPTIONS.md) and [future improvements](/docs/IMPROVEMENTS.md). The api is secured using token based authentication.

**The repository includes workflows that will do the following:**

- On pull request run the unit tests
- When pushing a tag with a `release-` prefix:
  - A [release](https://github.com/burger-mtbkr/customer-service/releases) will be created in the repository.
  - The image will be published to [docker hub](https://hub.docker.com/repository/docker/loanburger/customer-service/general)
  - The image will be published as a github [package](https://github.com/orgs/burger-mtbkr/packages?repo_name=customer-service)

### 1. How to test the api:

**Local debugging:**

- Clone this repository.
- Using [Visual Studio 2022 community](https://visualstudio.microsoft.com/vs/community/) : Open the Solution file under src/Customer/service.
- Select the docker Launch Profile and press play.
- The browser should open. You can then navigate to `http://localhost:<port>/swagger/index.html`.:eyes:

**Testing the api through Swaggers:**

_Note: you do not need to do this if you plan to use the [Customer SPA](https://github.com/burger-mtbkr/customer-spa)_

- The api is secured so you will need to signup to it.:lock:
- Go the `/api/signup` POST method and signup.
- After signing up you will receive a token in the response of your request
- Scroll to the top of the page and click the green Authorize button
- Enter: `Bearer <token>`. `<token>` being what was returned from your `/api/signup` request.
- After you have authenticated you can start creating customers then leads.

**Notes**:

- You only need to signup once - after that you can use the `api/login` endpoint to request a new token if required.
- If you prefer not to use Swagger I have included a [postman collection](/Postman/Customer%20Service.postman_collection.json) for the api.
