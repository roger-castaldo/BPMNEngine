# BPMEngine
A BPMN Engine written in .net.  The engine attempts to read in a bpmn notation xml document defining both the process(s) as well as the diagrams.  From here you can then load/unload the state,
render the diagram in its current state or animated into a gif.  Using the delegates for a process, you intercept and handle task and condition checking by reading additional xml held within flow and 
task objects.

# Future Features

- Linking to JINT to allow for script/service tasks to process javascript code for changing variables and not call an external delegate
- Using extendedElements to build .Net based code on the fly and have a callback made from the process

# Internal Conditions

Using bpmn:extensionElements inside a (bpmn:sequenceFlow,bpmn:process,bpmn:startEvent) you can specify conditions to be analyzed based on process variables and some basic comparisons by 
using a conditionSet xml tag with the following components inside.

## Comparison Components

Tags: isEqualCondition, greaterThanCondition, lessThanCondition, greaterThanOrEqualCondition, lessThanOrEqualCondition
Attributes: 
- leftVariable : used to indicate the name of the variable to place on the left side of the comparison
- rightVariable : used to indicate the name of the variable to place on the right side of the comparison
SubElements:
- left : used to indicate a constant value for the left side of the comparison
- right : used to indicate a constant value for the right side of the comparison
Rules: A tag can contain either a leftVariable or left element as well as either a rightVariable or right element

Example: 
<isEqualCondition leftVariable="username">
	<right>bob.loblaw</right>
</isEqualCondition>

## Negate Component

Used to negate the sub condition
Tag: notCondition
SubElements: andCondition, orCondition, isEqualCondition, greaterThanCondition, lessThanCondition, greaterThanOrEqualCondition, lessThanOrEqualCondition

Example:
<notCondition>
	<isEqualCondition leftVariable="username">
		<right>bob.loblaw</right>
	</isEqualCondition>
</notCondition>

## Null Check Component

Used to check if variable is null
Tag: isNull
Attributes: variable

Example:
<isNull variable="username"/>

## Comparison Collectors

Tags: andCondition, orCondition
Attributes: none
SubElements: andCondition, orCondition, notCondition, isEqualCondition, greaterThanCondition, lessThanCondition, greaterThanOrEqualCondition, lessThanOrEqualCondition

Example:
<andCondition>
	<isEqualCondition leftVariable="firstName">
		<right>bob</right>
	</isEqualCondition>	
	<isEqualCondition leftVariable="lastName">
		<right>loblaw</right>
	</isEqualCondition>
</andCondition>