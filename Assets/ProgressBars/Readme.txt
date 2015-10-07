1. Setup

Take a look at demo scenes for examples of setting everything up.

There are three kinds of bars:

- Basic: Rectangular bar that fills itself according to the minimum, maximum,
  and current value.
- Nonscaling: Similar to the first, but will stay the same size no matter where
  it is on the screen. Useful for interfaces where elements should not change
  size.
- Circular: A circular bar where the amount filled is indicated (typically) by
  a ring.

To create your own progress bar, you will need a background image and a fill
image. Border is also supported in basic and nonscaling bars. There is also a
"text" element that should be provided if you want the bar to display its
current value. Typically you'll want to create a GameObject with the progress
bar script of your choice as a component, and make it a child of your Canvas or
any other element (e.g. a panel). You can then add the graphics as children of
your new GameObject and rearrange them as you see fit. You need to link the
"Image" components to the progress bar script for it to be able to control them.

After you create the bar and link its Images, you can adjust their colours (they
will override the colour settings in individual Images, so be careful). Some
bars let you also set offset values for fine tuning if you find that some text
or image is misaligned.

2. Updating the bar

Controlling the bar is very easy and often does not require your attention at
all.

You can provide a component for the bar to track. If you do that, it will
automatically update all its value to match those in that component. To do so,
link your script in the inspector, and fill in the "Tracked Var Name", "Tracked
Var Max" and "Tracked Var Min" fields.

If you wish to control the bar manually, use its minValue, maxValue and
currentValue variables which are exposed and controlled inside the script (so
you can't for example make minValue larger than maxValue).

3. Text styles

There are three text styles for you to choose:

- Percent will divide current value by max and multiply by 100 to display it as
a percentage of maximum value 
- Current value displays the current value as it is without any modifications 
- Current slash max will display the value in the format of currentValue/maxValue

4. Help and support

If you're confused about any details try looking at example scenes and prefabs
we have included with the asset. All graphics and fonts are free for you to use
in all your projects.

If you're still unsure how something works, or you encounter any bugs or
problems, or you have a suggestion you would like to share, you can
always contact us via our e-mail or website:


greygolem@openmailbox.org
http://greygolem.tumblr.com/
