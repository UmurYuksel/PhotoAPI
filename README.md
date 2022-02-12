# PhotoAPI - IIASA Full-Stack Challenge

This readme includes quick installation info and technical details for better user experience.

1) Project Installation:

docker-compose and docker-engine versions may differ on your local machine, due to this reason, instead of letting visual studio to automatically compose the files, 
please follow the steps below:

     - Open the project in Viusal Studio 
     - Right Click on docker-compose 
     - Select "Open in Terminal" 
     - Type "docker-compose up" and hit the enter. (or docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d )
     - Now the contanier should be up and running. 
     - API will try broadcast itself through the port of :8002, If you go to this url: http://localhost:8002/index.html you can access to My API documentation and test the   application with swagger

Important: if the visual studio already composed the files in the moment when you opened the project with VS, Please remove the container by using your docker dekstop and reproduce the steps above.

2) Project Details

2.1 Introduction:
 
 After receiving the challenge on February 7, 2022. Even if it was a heavy development week in my current company, I have tried to develop an API that can meet your requirements.
 First, I have watched the PhotoQuest Go app video and tried to understand that what type of API can support this app in backend, then, I tried to create my modelling schema for this scenario.
 You can find the technical details below:

2.2 General Details: 

  This project was built in Visual Studio 2022 with .Net 6 Framework.
  Project uses Swagger for documentation, demonstration and test purposes.
  There are 3rd part libraries included for Image Processing. (e.g. MetadataExtractor, ImageSharp)
  The project uses volumes to persist its data in the docker environment. Images that were uploaded can be used next time when the container re-starts.
  The project also uses some configuration for limiting user operations such as IPRateLimiting etc.  

2.3 Project Specific Details:

  Assuming that the API will receive Image Files and again serve it back to PhotoQuest alike applications:

      -Implemented IP Rate Limiting for the requests that come from any ip for preventing the overuse of storage space and database.
      -A client can send max 10 requests within every 5 min. If the max request limit exceeded, the user will get an error like you've exceeded the max request size".
      -Post endpoint includes RequestSizeLimit equal to "bytes: 30_000=000" to prevent uploading an image which is bigger than the mentioned size.
      -API will only keep the original uploaded image in the storage and when the client request for resized image, original sized image will be processed, cloned and returned to the client in src compatible Base64 format.
      -Base64 strings can be too long, due to this fact, for fast querying and processing, I have implemented pagination for each get method that returns a list of photos to the client.
 
2.4 Docker related details: 
   
      -When the app composed, the API will try to connect on the depended mssql database until the database initialize itself and start listening on the port 1401. If API cannot connect to the Database, it will restart itself until
       the connection was successfully established.
