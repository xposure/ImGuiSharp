namespace ImGui
{
    // Temporary storage for outputting drawing commands out of order, used by ImDrawList::ChannelsSplit()
    // Draw channels are used by the Columns API to "split" the render list into different channels while building, so items of each column can be batched together.
    // You can also use them to simulate drawing layers and submit primitives in a different order than how they will be rendered.
    internal class ImDrawChannel
    {
        internal ImVector<ImDrawCmd> CmdBuffer { get; private set; } = new ImVector<ImDrawCmd>();
        internal ImVector<ImDrawIdx> IdxBuffer { get; private set; } = new ImVector<ImDrawIdx>();
    };
}
