# Duplicati Quick-Start Guide #

Welcome to the Duplicati Quick-Start guide!

This guide will help you setup backups that ensure that
your data is safe in the event of an accident.

## Where to store the data? ##
If your computer is stolen or broken, any backups stored
on the same computer will also be lost.

So to get protection from these events, you have to store
your backup somewhere else.

### An external harddisk or USB key ###
One option is to use an inexpensive external harddisk or USB key.
You can attach the device to your computer, and the backup can
be transfered to the external device.

This approach works, and provide very fast and cheap backup space.
But it requires that you remember to attach the remote device,
which may not be convinient if you have a laptop that you travel
with.

A more serious problem is that the device is physically near the
computer. In the case of theft, flood or fire, it is likely that
you loose both the computer and the external device.

Duplicati fully supports using external devices, but please
read the next section and decide if you can get a better solution
by storing the backups somewhere else.

### On another machine ###
A better approach is to store the backups on another machine,
somewhere far away from the machine you are protecting.

If you and a friend decide to exchange backups, you can get
remote storage for free. You may also have a friend that can
provide you with free storage space.

Duplicati itself is 100% free, but does not provide you with
a place to store such backups. Instead it allows you to use
a number of standards to connect to storage providers, so
you may choose the one you like the best.

If you can find no free storage options, you can use [Amazon S3](http://aws.amazon.com/s3/),
which is highly reliable and very cheap.
(see the [Amazon S3](http://aws.amazon.com/s3/) site for pricing).

## Duplicati Features ##
Duplicati enables you to secure your data by applying industrial
strength encryption and verification, no matter where they are
stored.

Duplicati ensures that only the parts of the files that are changed
are backed up, so the backups grow slowly, and take up much less
space.

Duplicati can seamlessly handle network outages, and still ensure
backup integrity.

## Getting started ##
Once you have decided where to store the data, you should get all
the required information from the provider, such as login credentials
and the servername or IP.

After you have that information, you are ready to use Duplicati.
Simply [download](http://code.google.com/p/duplicati/downloads/list) and install the program. Once done, Duplicati will
greet you with the backup wizard. Just follow the steps presented by
the wizard, and you will be up and running in no time.

[Click here to get a description of each of the individual steps](AddingBackup.md)