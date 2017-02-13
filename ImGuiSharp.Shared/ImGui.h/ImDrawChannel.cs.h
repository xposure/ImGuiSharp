// Draw channels are used by the Columns API to "split" the render list into different channels while building, so items of each column can be batched together.
// You can also use them to simulate drawing layers and submit primitives in a different order than how they will be rendered.
struct ImDrawChannel
{
	ImVector<ImDrawCmd>     CmdBuffer;
	ImVector<ImDrawIdx>     IdxBuffer;
};