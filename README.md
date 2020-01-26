# ElmaWebApiAtt
Application for certification tests in ELMA.
## Task
Need to develop an application to work with the WEB-API ELMA. The application should listen to the specified directory for changes. When creating a file in a directory, use the WEB-API ELMA to check for the presence of the file of the same name.
If such a file does not already exist on the system, upload file and create a document with this file. Otherwise, create a downloadable file and create a new version of the document.
## ElmaWebApi.App
Application executable module. Here is listening to directories and triggering change events. To configure the application, use the configuration file. Provides an interface for dynamic expansion (plugins).

It is possible to configure listening to several directories. Listener will ignore too frequent events directory changes specific to some file systems.

Can expand:
- Directory change event handling
- Register custom HttpClients and Singleton-services
- Additional external settings APIs
## ElmaWebApi.ElmaAPI
Library to work with ELMA WEB-API. Contains a convenient REST client. It contains a convenient REST client that includes the services necessary for working with documents, authorization services, and an entity manager. Automatically refresh ELMA tokens. 
## ElmaWebApi.ElmaFileWatcher
An example implementation of a handler. Performs the task set above, using the main application and the ELMA WEB-API library.
