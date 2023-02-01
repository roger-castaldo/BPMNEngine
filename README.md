# BPMEngine
A BPMN Engine written in .net.  The engine attempts to read in a bpmn notation xml document defining both the process(s) as well as the diagrams.  From here you can then load/unload the state,
render the diagram in its current state or animated into a gif.  Using the delegates for a process, you intercept and handle task and condition checking by reading additional xml held within flow and 
task objects.  Thanks to our authorized private software [HRForms.App](https://www.hrforms.app) for their continues support and testing.

Current Test Status:

[![.NET-Test-3x](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests3x.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests3x.yml)\
[![.NET-Test-3x-SkiaSharp-2.88.0](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests3x-Skia.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests3x-Skia.yml)\
[![.NET-Test-6x](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests6x.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests6x.yml)\
[![.NET-Test-6x-SkiaSharp-2.88.0](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests6x-Skia.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests6x-Skia.yml)\
[![.NET-Test-7x](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests7x.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests7x.yml)\
[![.NET-Test-7x-SkiaSharp-2.88.0](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests7x-Skia.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittests7x-Skia.yml)\
[![Jint-3.0.0-beta-1138](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittestsJint.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/unittestsJint.yml)\
[![CodeQL](https://github.com/roger-castaldo/BPMEngine/actions/workflows/codeql.yml/badge.svg?no-cache)](https://github.com/roger-castaldo/BPMEngine/actions/workflows/codeql.yml)\
[![codecov](https://codecov.io/gh/roger-castaldo/BPMEngine/branch/version2/graph/badge.svg?token=9TEAEN8OEC)](https://codecov.io/gh/roger-castaldo/BPMEngine)

# XSD

Adding xmlns:exts="https://raw.githubusercontent.com/roger-castaldo/BPMEngine/master/Extensions" to the bpmn:definitions will make things easier for accessing the defined xsd file for the project.  
This references all the extended elements defined below.

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

# Dynamic Internal Code

Executing Script tasks internally with dynamically compiled C#/VB.Net code.  Simple place an bpmn:extensionElements inside a script task and supply the following elements inside.
These script tags can also be used inside condition sets to return a boolean value for more complex condition checking.

Tags: cSharpScript (for C# code), VBScript (for VB.Net code)
SubElements: code, using, dll

Within the script element tag, either place the code (assuming there is no references required) or place the sub elements including 1 code element.  To acess the variables 
for the process at that point, the variable variables is passed into the code, which is an instance of ProcessVariablesContainer.  Any changes made to the variables object 
will be passed back into the system and the process variables updated for that step.

The using tag should contain a namespace reference as its value, the script tag will take care of the formatting in the code, for example: System.Data
The dll tag should contain a dll reference in its value, for example: System.Data.dll

Example:
<bpmn:scriptTask id="Task_1kqkg76" name="Close Request">
	<bpmn:extensionElements id="extTask_1kqkg76">
		<VBScript>
			variables("username") = "bob.loblaw"
		</VBScript>
	</bpmn:extensionElements>
	<bpmn:incoming>SequenceFlow_0xp0bq2</bpmn:incoming>
	<bpmn:outgoing>SequenceFlow_13xe9al</bpmn:outgoing>
</bpmn:scriptTask>

The example above will set the variable "username" to "bob.loblaw" when the script task is called.

# Javascript Code

Executing Javascript code internally using the variables parameter.  This is done by dynamically loading JINT to process javasacript inside .Net.  These can be used for both script tasks 
as well as condition checking.

Tag: Javascript

Within the script element tag, you must supply the javascript code to be run.

Example:
<Javascript>
	variables["username"] = "bob.loblaw"
</Javascript>
