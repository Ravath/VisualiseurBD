# VisualiseurBD
Displays pictures and navigates through the pictures in a folder

## Why use Visualizer
1) First, it behaves in an optimal way for reading scans, and especially the webtoon format. That is, the zoom fits image width and not image height.
2) Every navigation shortcut can be used with only the left hand, for maximum convenience. Leave your right hand on the mouse and master your navigation efficiently!

### Suported formats
bmp, gif, ico, jpg, jpeg, png, wdp, tiff, webp

## How to use
### Installation
1) Have the executable on your computer by downloading it or compiling from source.
2) Right-click on picture files > Open With > Select the executable.
3) Repeat for every file format (JPEG, JPG, PNG, webp, etc.)

### Navigation
#### This is azerty bihatch
- q/d : previous/next picture. Notice that windows ordering ond VisualiseurBD ordering might differ. The application will restart from the first picture when reaching the end.
- z/s : top/down of the picture
- \<Space\> : Scrolling down. No scrolling up shortcut.
- a : Fits the image width. Maximum is limited by screen width. 
- e : Fits the image height. Maximum is limited by screen height. 
- f : Fits full screen width.
- o : Open file search to browse for a picture to open.
- r : jumps to a random image from the picture folder.
- c : start/stop a drawing practice session (see below).

#### Drawing practice session
When the drawing session is activated, it will display a sequence of pictures for a limited time, poses drawing. (See also [SketchDaily](http://reference.sketchdaily.net/))

##### Starting
Use 'c' to start.

Before that, use 'r' to start from a random picture from the same folder.

##### Configuration
The application uses as default configuration the standard practice times.

For changing this configuration, create a CSV file 'course.csv' in the application's directory. Enter the session steps in rows, with first the duration in seconds and then the number of practices, separated by a semi-column.

Example with default values.
```
30;10
60;5
300;2
600;1
```