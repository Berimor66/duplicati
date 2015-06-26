# File types used in Duplicati #

Duplicati uses three different types of files: manifest, content, and signature.

The manifest files are small control files that contain a cryptographic checksum for the other files. This checksum protects against malicious or accidental data change.

The content files contain the actual file data, or the changed portions of files. Content files are the largest of the files.

The signature files describes what each file looked like at the time of the last backup. By examining a file and comparing it to it's previous signature, Duplicati can figure out what parts of a file were changed. Signature files are small compared to content files, usually less than 5% of the content file size.

# The signature cache functionality #

When Duplicati needs to perform an incremental backup, it needs access to the signature files from the previous backup. Since those files were most likely created on the machine that needs them, Duplicati features a signature cache. After a signature file has been uploaded to the remote destination, Duplicati copies it to the signature cache folder.

When the signature file is needed later, Duplicati first checks to see if the file is in the signature cache. If the file is found, the content is compared to the checksum found in the manifest file, which ensures that the local and remote file are identical.

If a local valid file exists, Duplicati does not download. If no file exists, or the file is invalid, Duplicati downloads the file and stores a copy.

If you need all the disk space you can get, you can disable the signature cache completely.

# Benefits and drawbacks #
The primary benefit of the signature cache is to reduce the number of remote requests, as well as the amount of data transfered when making a backup.

Duplicati can also use the cached files for other purposes, such as listing files in the backup, which then performs much faster.

The drawback of the signature cache is use of disk space on the machine that runs the backup.

# Clearing the signature cache #
Whenever a signature file is deleted on the remote machine, it is also removed from the signature cache, so there should be no leftover files in the signature cache.

If you suspect that there are unused files, and you need the extra space, it is perfectly safe to clear the signature cache. When the files are needed, Duplicati will just download them again.

Under normal operation, Duplicati only uses the most recent backup chain, and thus only the signature files from this chain. If you clear the signature cache, it may not grow to the same size as it was before, but that does not mean that there were leftover files, just that some files were not yet needed. If you never interact with the previous backup chains, those files will never be downloaded.