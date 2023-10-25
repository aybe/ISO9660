Basically, for one to correctly parse these non-compliant, retard Linux .ISO images:

- read the first directory record, in system area, look for SP and CE entries if any
- if found, CE will point to a sector where the fucking Rock Ridge version string is
- according to whether we have that information or not, we can read strings properly

Supposes major refactoring for ZERO outcome, as we're able to handle it differently.

References:

IEEE P1281 SYSTEM USE SHARING PROTOCOL DRAFT STANDARD VERSION 1.12
IEEE P1282 ROCK RIDGE INTERCHANGE PROTOCOL DRAFT STANDARD VERSION 1.12 