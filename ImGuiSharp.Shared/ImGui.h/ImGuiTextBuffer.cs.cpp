//-----------------------------------------------------------------------------
// ImGuiTextBuffer
//-----------------------------------------------------------------------------

// On some platform vsnprintf() takes va_list by reference and modifies it.
// va_copy is the 'correct' way to copy a va_list but Visual Studio prior to 2013 doesn't have it.
#ifndef va_copy
#define va_copy(dest, src) (dest = src)
#endif

// Helper: Text buffer for logging/accumulating text
void ImGuiTextBuffer::appendv(const char* fmt, va_list args)
{
	va_list args_copy;
	va_copy(args_copy, args);

	int len = vsnprintf(NULL, 0, fmt, args);         // FIXME-OPT: could do a first pass write attempt, likely successful on first pass.
	if (len <= 0)
		return;

	const int write_off = Buf.Size;
	const int needed_sz = write_off + len;
	if (write_off + len >= Buf.Capacity)
	{
		int double_capacity = Buf.Capacity * 2;
		Buf.reserve(needed_sz > double_capacity ? needed_sz : double_capacity);
	}

	Buf.resize(needed_sz);
	ImFormatStringV(&Buf[write_off] - 1, len + 1, fmt, args_copy);
}

void ImGuiTextBuffer::append(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	appendv(fmt, args);
	va_end(args);
}