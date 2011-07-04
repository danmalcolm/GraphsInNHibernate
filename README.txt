MAPPING GRAPH STRUCTURES IN NHIBERNATE

____________
INTRODUCTION

Looking at options for mapping graph structures in NHibernate.

Will include:

Model
Various mapping options
Efficient graph retrieval etc

Just playin' around really. I'm sort of expecting to find that it can't be done that well, which can justify looking further into graph databases.

__________
SETTING UP

You'll need to do the following after cloning the project:

* Review the connection string in the app.config. Change the server name if you don't want to run against local sqlexpress.
* Create a new empty database called GraphsInNHibernate.Tests
* Running the database tests will (re)create the database objects. Changing the mappings can cause the schema generation script to fail with a SqlException like "There is already an object named 'XXX' in the database". The simplest thing to do here is to drop and recreate the database.


____
TODO

RelatedNode