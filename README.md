# image-recognition

Gathering image objects and other details using Microsoft Cognitive Service.

## An Analyzed Image
<img style="height:50%" src="https://raw.githubusercontent.com/TareqNewazShahriar/image-recognition/main/results/a%20room%20with%20a%20table%20chairs%20and%20a%20large%20window.jpg" />

## Detail Analysis Data
```
Summary:
"a room with a table chairs and a large window" with confidence 0.367992639541626

Categories:
"abstract_" (confidence: 0.00390625)
"indoor_venue" (confidence: 0.48828125)

Tags:
"ceiling" (confidence: 0.9870719313621521)
"furniture" (confidence: 0.9845324754714966)
"indoor" (confidence: 0.9806315898895264)
"coffee table" (confidence: 0.9568498730659485)
"chair" (confidence: 0.9361237287521362)
"floor" (confidence: 0.9341132640838623)
"kitchen & dining room table" (confidence: 0.9336949586868286)
"interior design" (confidence: 0.919575572013855)
"couch" (confidence: 0.9066134095191956)
"wall" (confidence: 0.8842116594314575)
"studio couch" (confidence: 0.8454673290252686)
"window" (confidence: 0.768425464630127)
"restaurant" (confidence: 0.6770898699760437)
"dining" (confidence: 0.6709968447685242)
"room" (confidence: 0.5848329067230225)
"table" (confidence: 0.550926148891449)
"hotel" (confidence: 0.47121208906173706)

Objects:
"chair" with confidence 0.803 at location 14, 333, 850, 1239
"table" with confidence 0.503 at location 1077, 1437, 924, 1155
"couch" with confidence 0.555 at location 1717, 1910, 945, 1430
"dining table" with confidence 0.702 at location 757, 1485, 1104, 1424
JSON:
[{"Rectangle":{"X":14,"Y":850,"W":319,"H":389},"ObjectProperty":"chair","Confidence":0.803,"Parent":{"ObjectProperty":"seating","Confidence":0.806,"Parent":null}},{"Rectangle":{"X":1077,"Y":924,"W":360,"H":231},"ObjectProperty":"table","Confidence":0.503,"Parent":null},{"Rectangle":{"X":1717,"Y":945,"W":193,"H":485},"ObjectProperty":"couch","Confidence":0.555,"Parent":{"ObjectProperty":"seating","Confidence":0.705,"Parent":null}},{"Rectangle":{"X":757,"Y":1104,"W":728,"H":320},"ObjectProperty":"dining table","Confidence":0.702,"Parent":{"ObjectProperty":"table","Confidence":0.751,"Parent":null}}]

Adult:
Has adult content: "False" with confidence 0.00110627303365618
Has racy content: "False" with confidence 0.0019235885702073574
Has gory content: "False" with confidence 0.000871725904289633

Color Scheme:
Is black and white?: False
Accent color: A1662A
Dominant background color: Grey
Dominant foreground color: Grey
Dominant colors: Grey,Black

Image Type:
Clip Art Type: 0
Line Drawing Type: 0
```
