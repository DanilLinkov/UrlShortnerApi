# URL Shortner API

## Table of contents

1. [Description](#description)
2. [Purpose & Quick feature summary](#purpose--quick-feature-summary)
3. [Tech stack](#tech-stack)
4. [Install & Run](#install--run)
5. [Usage](#usage)
6. [SQL database tables](#sql-database-tables)
7. [Session based Authentication & Anonymous user identification](#session-based-authentication--anonymous-user-identification)
8. [Key generation](#key-generation)
9. [Caching](#caching)
10. [Clean up of expired short URLs](#Clean-up-of-expired-short-URLs)

## Description
### Note: for a more visual representation of the functionality explained here please visit a hosted front-end React app here https://shorturlclient.azurewebsites.net/app, also mentioned below along with the source code.

URL shortner API is a .net core web API that shortens provided URLs, stores them in the database and returns an 8 character key of the long URL that can be used to retrieve it. The API allows for full CRUD operations on the shortened URLs (If all authentication & authorization checks pass). Session based Authentication (using Redis) is used and is made to allow for anonymous and authenticated user identification.

This API is currently hosted on Azure with this URL: 

- Azure hosted instance: https://shorturlapi.azurewebsites.net/

The code and hosted URL for the front-end React app can be found here:  

- Source code and documentation: https://github.com/DanilLinkov/UrlShortnerClient

- Azure hosted instance: https://shorturlclient.azurewebsites.net/app

This project also uses a Key Generation Service (personal project created for this) which can be found here: 

- Source code and documentation: https://github.com/DanilLinkov/KeyGenerationService
- Azure hosted instance: https://kgsapikeyusagefunction.azurewebsites.net/

## Purpose & Quick feature summary

The primary purpose of this API is to store long URLs such as long image links that are sometimes 200+ characters long and to be able to access them with a much shorter URL which is more readable, presentable and sharable. This API is used in a front-end React app linked above where all the functionality can be visually seen.

- Takes in long URLs as input and stores them and returns a unique 8 character Id (also allows for custom Ids) which can be used to access the stored long URL.
- Uses session based authentication combined with Redis which allows for anonymous and authenticated user identification and stores the encrypted session cookie in the user's browser along with an encrypted GUID cookie if the user is anonymous.
- ShortUrl keys are generated using a Key Generation Service for which the github link can be found above.
- Hot / Recently accessed ShortUrls are cached for quicker response times for other user's trying to access them.
- Expired ShortUrls are removed through the use of an Azure function that runs a store procedure and any keys generated by Key Generation Service are returned back to it so that they can be used again.
- Hosted on Azure with the URL provided above and used by the front-end app also linked above.

## Tech stack

- .net core 5.0
- .net core Identity for authentication
- Redis for caching sessions and recently used URLs
- SQL database
- Azure for hosting
- Azure function that runs a store procedure for removing expired URLs
- Key generation service (created for this) linked above

## Install & Run

- ` git clone https://github.com/DanilLinkov/UrlShortner.git`
- In a command window at the root level of the project run `dotnet restore` followed by `dotnet run`

## Usage

Not all the possible requests / responses are shown here but only the mains ones. Therefore error responses and other special cases were omitted.

For information about the cookies created and used when using these endpoints please read the [authentication](#Session based Authentication & Anonymous user identification) section below.

### Authentication endpoints

1. `POST /api/login`

   Used for logging in as a user.

   Also returns and sets a session cookie in the user's browser.

   

   Request JSON:

   ```json
   {
     "username": "string",
     "password": "string"
   }
   ```

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "message": "string",
     "result": {
        // If valid login then username is also returned
       "userName": "string"
     }
   }
   ```

1. `GET /api/login`

   Used for checking whether a user is logged in by checking if their session is valid and is of a registered user

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "message": "string",
     "result": {
        // If valid session then username is also returned
       "userName": "string"
     }
   }
   ```

   

2. `POST /api/register`

   Used for registering a user.

   Also returns and sets a session cookie in the user's browser.

   

   Request JSON:

   ```json
   {
     "username": "string",
     "password": "string"
   }
   ```

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "message": "string"
   }
   ```

   

3. `POST /api/logout`

   Used for logging a user out by deleting their session cookie in the browser.

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "message": "string"
   }
   ```

   

### ShortUrl endpoints

1. `GET /api`

   Returns all of the created Short URLs based on the cookies in the request

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "result": [
       {
         "longUrl": "Provided long URL",
         "shortenedUrlId": "CustomId or uniquely generated 8 character Id",
         "creationDate": "Date time",
         "expirationDate": "Date time",
         "uses": "int"
       }
     ]
   }
   ```

   

2. `POST /api`

   Creates a new ShortUrl if valid data is provided and assigns ownership of it to the user based on the cookies in the request

   Request JSON:

   ```json
   {
     "customId": "string or null",
     "longUrl": "valid http or https URL",
     "expirationDate": "Date time"
   }
   ```

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "result": {
       "longUrl": "Provided long URL",
       "shortenedUrlId": "CustomId or uniquely generated 8 character Id",
       "creationDate": "Date time",
       "expirationDate": "Date time",
       "uses": "int"
     }
   }
   ```

   

3. `PUT /api`

   Updates an existing ShortUrl if valid data is provided and the user owns it.

   Request JSON:

   ```json
   {
     "shortenedUrlId": "CustomId or uniquely generated 8 character Id",
     "longUrl": "valid http or https URL",
     "expirationDate": "Date time"
   }
   ```

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "result": {
       "longUrl": "Provided long URL",
       "shortenedUrlId": "CustomId or uniquely generated 8 character Id",
       "creationDate": "Date time",
       "expirationDate": "Date time",
       "uses": "int"
     }
   }
   ```

   

4. `DELETE /api`

   Deletes an existing ShortUrl if valid data is provided and the user owns it.

   Request JSON:

   ```json
   {
     "shortenedUrlId": "CustomId or uniquely generated 8 character Id of the shortned URL to delete"
   }
   ```

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "result": ["List of currently owned Short Urls"]
   }
   ```

   

5. `GET /api/{shortUrl}`

   Returns the ShortUrl object with the given Id which can be used to access the long URL it was shortening.

   Response JSON:

   ```json
   {
     "statusCode": "int",
     "result": {
       "longUrl": "Provided long URL",
       "shortenedUrlId": "CustomId or uniquely generated 8 character Id",
       "creationDate": "Date time",
       "expirationDate": "Date time",
       "uses": "int"
     }
   }
   ```

   

## SQL database tables

- ShortUrls table
  - PK: Id - int
  - LongUrl - nvarchar
  - SK: ShortenedUrlId - nvarchar
  - ExpirationDate - datetime
  - UserId - uniqueidentifier
  - Uses - int
  - CreationDate - datetime

Default AspNet Identity tables which are AspNetUserTokens, AspNetUsers, AspNetUserRoles, AspNetUserLogins, AspNetUserClaims, AspNetRoles and AspNetRoleClaims.

## Session based Authentication & Anonymous user identification

Note: Username and password requirements were set to minimum by default for easy testing and playing around with the API.

There are two cookies used for the authentication functionality of this API and they are:

### 1. ShortUrl-SessionId

Used for storing the user's session Id that is then used by the API to identify the user and also allows to store information about a user's session. Redis is used to hold the actual user authentication ticket as well as any other information. In the case of anonymous user's their session contains their uniquely generated GUID and any other information that could be useful about their session. This session cookie and the Redis session are both set to last for 30 minutes and are sliding expiration therefore could potentially last longer than that if the user continuously uses the API.

Note: All of the cookies and encrypted so the user is unable to interpret any of the information on the cookies.

### 2. ShortUrl-GUID

Used to identify anonymous user's, generated by the API and sent to the user whenever they try to access any of the endpoints without this cookie. This cookie is also set to last for 5 years therefore anonymous user's never need to register in order to use the application and to have their ShortUrls saved. 

Authentication for registered user's is done using .net core identity which was made to use a custom Authentication Ticket store in order to store them in Redis so only the session Id is sent back to the user. For anonymous user's a similar implementation is used to also only send the session Id back to them however they must always have a ShortUrl-GUID cookie as described above in order to always be able to identify them between different sessions.

A middleware was also created which is hit on every request and it attaches any of the two cookies if they are not already present depending on whether the user is authenticated or is an anonymous user.

On logout the user's session cookie as well as the information about them stored on Redis are deleted.

If an anonymous user creates ShortUrls and then registers / logs in then the ownership of those ShortUrls is transferred to this existing user if the logout they will no longer be able to access them with the same anonymous ShortUrl-GUID.

## Key generation

For each ShortUrl if a unique custom Id is not provided then a new 8 character unique Id is used using a Key Generation Service https://github.com/DanilLinkov/KeyGenerationService (personal project created for this). These keys are globally unique therefore if multiple instances of this API were made then they would each receive unique keys and not have to worry about duplicate keys being formed. More info on it in the github link provided above.

## Caching

Apart from caching the user's session data, recently accessed ShortUrls are also cached for 30 minutes using Least-Recently-Used eviction policy in case the cache ever fills up. Therefore if a user accesses a ShortUrl that has been accessed in the last 30 minutes then their request would be fulfilled from the cache improving the performance of hot ShortUrls.

## Clean up of expired short URLs

Expired ShortUrls will not be returned to the user's and will also be cleaned up via a store procedure which is currently run every 24 hours using an Azure function.

