# BPMEngine
A BPMN Engine written in .net.  The engine attempts to read in a bpmn notation xml document defining both the process(s) as well as the diagrams.  From here you can then load/unload the state,
render the diagram in its current state or animated into a gif.  Using the delegates for a process, you intercept and handle task and condition checking by reading additional xml held within flow and 
task objects.

# Future Features

- A built in condition engine to analize path conditions based on variables supplied, falling back on external delegates if the system cannot process information
- Linking to JINT to allow for script/service tasks to process javascript code for changing variables and not call an external delegate
- Using extendedElements to build .Net based code on the fly and have a callback made from the process
