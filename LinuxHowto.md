# Requirements #

Duplicati runs under Mono, and is tested with Ubuntu 8.10 through 11.10.
It also runs on all major distributions, such as LinuxMint, Debian, Fedora and OpenSuSE.

From version 1.3 there are native packages available for all the above mentioned systems, and a generic .tgz package.

To install, simply download the package and install it using your normal package manager.

After installation, you can start Duplicati from your menu or by running "duplicati" from a terminal. You can access the commandline version by running "duplicati.commandline" from a terminal.

# Installing Mono #
If you use the .deb file, all dependencies should be installed automatically, if not you can run the following command on Debian, Ubuntu or LinuxMint:
```
sudo apt-get install mono-runtime libmono2.0-cil libmono-winforms2.0-cil expect
```

If you are using Fedora, you need to install Mono manually:
```
sudo yum install mono-basic mono-winforms expect
```

If you are using OpenSuSE, you need to install Mono manually:
```
sudo yast -i mono-basic mono-winforms expect
```

# Manual install #
If you are unable to install using the .deb or .rpm, you can use the .tgz package:
```
cd /
wget "http://duplicati.googlecode.com/files/duplicati-1.3.tgz"
sudo tar -xvf "duplicati-1.3.tgz"
```

This will essentially do the same as installing either the .deb or the .rpm package. You get a GNOME menu shortcut and launcher scripts in /usr/bin/duplicati and /usr/bin/duplicati.commandline.

You need to manually install Mono+Winforms (>=2.6), libsqlite (>=3.6.12) and expect (only for using unmanaged SSH option).

You can use your system package manager, like described above, or use these download links:
  * [Mono and Mono-Winforms](http://www.go-mono.com/mono-downloads/download.html)
  * [libsqlite](http://www.sqlite.org/download.html)
  * [expect](http://sourceforge.net/projects/expect/files/Expect/)

## The fully manual way ##
If there is no package available or you just want to try Duplicati without messing with your system (i.e. portable install), you can use the .zip package. You must have Mono installed for this to work (see description above for manual install).

If you want to do this the fully manual way, try this:
```
wget "http://duplicati.googlecode.com/files/Duplicati%201.3.zip"
unzip "Duplicati 1.3.zip"
cd "Duplicati"
```

You can now start Duplicati:
```
mono Duplicati.exe
```

You can also use the commandline component of Duplicati, just by running it:
```
mono Duplicati.Commandline.exe
```

These instructions use the generic binaries, and works with any version of Mono > 2.6. The same binaries work if you are running either Mono or MS.Net on Windows.

Using the .zip file does not install a menu item, and does not provide launchers in /usr/bin.

# Known Issues #

These are know issues that limit functionality or requires special action on behalf of the user. Also read the user comments below to see if any users have found other problems.

## Issues with SSH ##
1.0 Final has some issues with the SSH backend, those are fixed in newer versions, see the [wiki:Downloads page].

## Issues with CloudFiles, Skydrive, GoogleDocs and S3 ##
CloudFiles, Skydrive, GoogleDocs and S3 use SSL which requires you to trust their certificate issuer. Run this command to let Mono use the same certificates that Mozilla (Firefox) uses:
```
mozroots --import --sync
```

If your version of Mono is missing the mozroots application, you can [download mozroots.exe](http://www.2shared.com/file/Zg7H-IBV/mozroots.html) and then run it like this:
```
mono mozroots.exe --import --sync
```



## Using a newer SQLite version ##
If Duplicati complains about an incompatible Sqlite version, do this once (not required for Ubuntu 9.04 and newer):
```
chmod +x StartDuplicati.sh
```

You can now use the wrapper to start Duplicati:
```
./StartDuplicati.sh
```

This will use the supplied libsqlite3.so.0 which is the official "SQLite3 3.6.12" compiled for x86. If you use another architecture, you must get the appropriate binary, either from the [sqlite website](http://www.sqlite.org), or you linux distributions repository.

Note that these files are only found in the .zip version, not in any of the other packages.

## Using LVM snapshots ##
As of Duplicati 1.0.1 `r465` there is now a "snapshot-policy" option that uses LVM snapshots. If you use LVM, you can activate this feature by setting the option "snapshot-policy" to "required", either on the commandline, or in the advanced options.

The function relies on the scripts located in the "lvm-scripts" subfolder of the installation. Before activating the feature, make sure that the scripts are executable by executing (only required if you run from a .zip based install):
```
cd lvm-scripts
chmod +x *.sh
```

The LVM snapshot feature requires that there is some free space in the LVM volume group. You can edit the scripts in the "lvm-scripts" folder to better fit your needs. You can [read more about LVM on their website](http://sourceware.org/lvm2/).

The LVM commands require that the user has sufficient privileges (e.g. root).

Note that Ubuntu 10.04 standard install does not use LVM, but the alternative installer has an option to use LVM (be sure not to use the entire disk, but leave a few megabytes for snapshot data). Also note that when mounting the snapshots, Nautilus will display the folder as it thinks it is a removable disk. You can [disable the "Browse removable media" option to prevent this](http://ubuntuforums.org/showthread.php?t=392167).