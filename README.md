# ISO9660

Extracts files/tracks from CD images.

## Introduction

There are a lot of compact disc image reading libraries out there on the web.

However, none appear able to do the simple task of reading a file in RAW mode.

Such ability allows extracting *special* files, e.g. movies in XA CD-ROMs or VideoCDs.

## Overview

This project is comprised of the following:

- cue sheet parser (GoldenHawk's CDRWIN .cue format)
- physical and logical structures (disc, track, sector, file system)
- command-line interface application for listing/reading files/tracks
