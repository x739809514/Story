# StoryEditor

This is a story tool based on XNode for Unity developer. It has 8 kinds of nodes:
1. StartNode
2. AssetNode
3. BackgroundNode
4. AudioNode
5. DialogNode
6. AnimNode
7. OptionNode
8. EndNode

## How to use

1. Drag the prefab `StoryCanvas` to scene ![image](https://github.com/user-attachments/assets/c5b635eb-28d2-468b-a75c-f4c0ddde4d2d)
2. Create a GameObject, named `GameManager`, add two components: `GameManager`, `StorySystem` ![image](https://github.com/user-attachments/assets/591607f2-caa0-4f19-a54e-0c0f17739da9)
3. In projects, `create->DialogAsset`, then add some character names; `create->DialogGraph`, and double click to open it. ![image](https://github.com/user-attachments/assets/8d337530-ea69-489a-b2e0-1ce3c0f352d2)
4. create first node `AssetNode`, drag the dialogAsset to it ![image](https://github.com/user-attachments/assets/3e1ba025-f907-4571-a48e-99897efe64c5)
5. create `StartNode` and `DialogNode`, match them together
6. edit `DialogNode`, choose name, character icon, dialogue type and dialogue content, set the last dialogue item with the type `GoNext` ![image](https://github.com/user-attachments/assets/861df608-bfe2-4dbb-ba24-0bd407aa101a)

7. add the `EndNode`, match `DialogNode` with it
8. A simple dialogGraph is finished.
9. Drag your `dialoggraph` to the GameObject `GameManager` ![image](https://github.com/user-attachments/assets/8e043744-e37f-4b16-81b9-fc0957090d62)

10. run the game

## Nodes

StartNode: A graph has and only has one StartNode

AssetNode: Store character Asset

BackgroundNode: Set the background image if you want

AudioNode: Store all audio assets you want in this node, and you can use them via `audioManager.soundDic`

DialogNode: Add dialogues into it. There are 5 types: Null, Normal, CEvent, Option, GoNext
1. Null: Do nothing
2. Normal: the normal chat mode
3. CEvent: you can send event to outside
4. Option: if you want to add choice in your story, you can use it, and you use option type, the next node must be `OptionNode`
5. GoNext: if you want to end this part of dialogue, you can choose this type in the end
Attention: The `Option` or `GoNext` only can be used when their item is the last one in the node

AnimNode: if you want to play animation you can use this

OptionNode: you can set the specific options in this node

EndNode: if you want to end this dialog graph, use this node

## StorySystem

There is a list, you can add all dialog graph in that. they will be played one by one

## GameManager, AnimPlayable
The manager of whole game system, and there is an `audiomanager` in it; AnimPlayable is used to play animation
