![http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page6d.png](http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page6d.png)

On this page you must select where to store the backups.

The AWS id and key is avalible from your Amazon S3 account page.
Click the link to create an account, or log in to retrieve the required information.

On S3 all data is stored in buckets, and each bucket must have a unique name. If you enter a name, Duplicati will ask you if it should be prefixed with your AWS id, as that will make the bucket name unique. For more information on buckets, see the Amazon S3 help pages.

The test button will test the connection, and thus reduce the chance that the information contains errors. If the bucket is not created, the test will fail, but the first backup will create the bucket for you.

The option to use a european server, only applies if Duplicati creates the bucket for you. If the bucket already exists, this option has no effect.

When you are done, click the "Next >" button. If you have not already tested the connection, Duplicati will ask you if it should be tested before you continue.

[<< Go to previous page](AddPage5.md) - [Go to next page >>](AddPage7.md)
