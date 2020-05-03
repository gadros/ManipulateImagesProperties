old picture meta-data reading and updating<br />
set meta-data to old images so they will automatically arrange when uploading them google photos<br />
<p class="auto-style1">
    <strong>Description:</strong></p>
<p class="auto-style3">
    a .Net console application that operates on image files in a folder hierarchy and attempts to set the &#39;Date Taken&#39; and &#39;Subject&#39; (and &#39;Title&#39;) attributes of the image based on the file and folder name (setting the value for &#39;Subject&#39; is only based on the file name). hopefully, values aren&#39;t modified if it fails to &#39;make sense&#39; based on these.
    <br/>
    <br/>
The need came when having many old images prior to those taken with a digital camera and wanting to upload them to google photos and have them arranged automatically based on meta-data in the image.
    <br/>
    <br/>
The application uses ExifTool.exe to actually read and update the attributes of an image file (how should i put it, i'm not even scratching on the possiblities or the correct usage of this tool...). Thank you!!
</p>
<p class="auto-style2">
    open issues:
</p>
<ol>
    <li>Character encoding - need to be tested with other languages (tested only with Hebrew)</li>
    <li>Currently it works from the command-line to support operating on a folder hierarchy. need to support a windowed mode where you could see/preview the image and allow to manually set the attributes.</li>
    <li>
        <p>
            Performance - the application runs a ExifTool process per each query or edit of an image file. one direction is to keep the ExifTool process open and execute multiple operations
        </p>
    </li>
    <li>
        <p>
            Tests - not sure about the coverage and mostly it is that they are very much at integration level truly doing heavy I/O operations
        </p>
    </li>
    <li>
        <p>
            setting the title of the image
        </p>
        <ol>
            <li>
                <p>
                    should become optional
                </p>
            </li>
            <li>
                <p>
                    the same value is set to: title, description, EXIF:XPTitle and EXIF:XPSubject
                </p>
            </li>
            <li>
                <p>
                    the logic of it might not fit all or actually bring value</p>
            </li>
        </ol>
    </li>
    <li>
        <p>
            revisit logging - maybe too much and not ineffective?</p>
    </li>
</ol>

<p>
    &nbsp;</p>
<p>
    <span class="auto-style1">future directions (assuming the above are handled)</span>:</p>
<ol>
    <li>
        <p>
            sometimes an image has a 'date taken' value, only that the digital camera's date is wrong. implement an easy way to drop (or accept) many image files and clear this value. so that a subsequent run will calculate and set them (because currently any value > 1/1/1800 is honored and not updated.
        </p>
    </li>
    <li>
        <p>
            become an add-in to IrfanViewer
        </p>
    </li>
    <li>
        <p>
            add an interactive mode with GUI to see and manually manipulate an image
        </p>
    </li>
    <li>
        <p>
            make it cross platform
        </p>
    </li>
</ol>
