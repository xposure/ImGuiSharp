//-----------------------------------------------------------------------------
// ImText* helpers
//-----------------------------------------------------------------------------

// Convert UTF-8 to 32-bits character, process single character input.
// Based on stb_from_utf8() from github.com/nothings/stb/
// We handle UTF-8 decoding error by skipping forward.
int ImTextCharFromUtf8(unsigned int* out_char, const char* in_text, const char* in_text_end)
{
	unsigned int c = (unsigned int)-1;
	const unsigned char* str = (const unsigned char*)in_text;
	if (!(*str & 0x80))
	{
		c = (unsigned int)(*str++);
		*out_char = c;
		return 1;
	}
	if ((*str & 0xe0) == 0xc0)
	{
		*out_char = 0xFFFD; // will be invalid but not end of string
		if (in_text_end && in_text_end - (const char*)str < 2) return 1;
		if (*str < 0xc2) return 2;
		c = (unsigned int)((*str++ & 0x1f) << 6);
		if ((*str & 0xc0) != 0x80) return 2;
		c += (*str++ & 0x3f);
		*out_char = c;
		return 2;
	}
	if ((*str & 0xf0) == 0xe0)
	{
		*out_char = 0xFFFD; // will be invalid but not end of string
		if (in_text_end && in_text_end - (const char*)str < 3) return 1;
		if (*str == 0xe0 && (str[1] < 0xa0 || str[1] > 0xbf)) return 3;
		if (*str == 0xed && str[1] > 0x9f) return 3; // str[1] < 0x80 is checked below
		c = (unsigned int)((*str++ & 0x0f) << 12);
		if ((*str & 0xc0) != 0x80) return 3;
		c += (unsigned int)((*str++ & 0x3f) << 6);
		if ((*str & 0xc0) != 0x80) return 3;
		c += (*str++ & 0x3f);
		*out_char = c;
		return 3;
	}
	if ((*str & 0xf8) == 0xf0)
	{
		*out_char = 0xFFFD; // will be invalid but not end of string
		if (in_text_end && in_text_end - (const char*)str < 4) return 1;
		if (*str > 0xf4) return 4;
		if (*str == 0xf0 && (str[1] < 0x90 || str[1] > 0xbf)) return 4;
		if (*str == 0xf4 && str[1] > 0x8f) return 4; // str[1] < 0x80 is checked below
		c = (unsigned int)((*str++ & 0x07) << 18);
		if ((*str & 0xc0) != 0x80) return 4;
		c += (unsigned int)((*str++ & 0x3f) << 12);
		if ((*str & 0xc0) != 0x80) return 4;
		c += (unsigned int)((*str++ & 0x3f) << 6);
		if ((*str & 0xc0) != 0x80) return 4;
		c += (*str++ & 0x3f);
		// utf-8 encodings of values used in surrogate pairs are invalid
		if ((c & 0xFFFFF800) == 0xD800) return 4;
		*out_char = c;
		return 4;
	}
	*out_char = 0;
	return 0;
}

int ImTextStrFromUtf8(ImWchar* buf, int buf_size, const char* in_text, const char* in_text_end, const char** in_text_remaining)
{
	ImWchar* buf_out = buf;
	ImWchar* buf_end = buf + buf_size;
	while (buf_out < buf_end - 1 && (!in_text_end || in_text < in_text_end) && *in_text)
	{
		unsigned int c;
		in_text += ImTextCharFromUtf8(&c, in_text, in_text_end);
		if (c == 0)
			break;
		if (c < 0x10000)    // FIXME: Losing characters that don't fit in 2 bytes
			*buf_out++ = (ImWchar)c;
	}
	*buf_out = 0;
	if (in_text_remaining)
		*in_text_remaining = in_text;
	return (int)(buf_out - buf);
}

int ImTextCountCharsFromUtf8(const char* in_text, const char* in_text_end)
{
	int char_count = 0;
	while ((!in_text_end || in_text < in_text_end) && *in_text)
	{
		unsigned int c;
		in_text += ImTextCharFromUtf8(&c, in_text, in_text_end);
		if (c == 0)
			break;
		if (c < 0x10000)
			char_count++;
	}
	return char_count;
}

// Based on stb_to_utf8() from github.com/nothings/stb/
static inline int ImTextCharToUtf8(char* buf, int buf_size, unsigned int c)
{
	if (c < 0x80)
	{
		buf[0] = (char)c;
		return 1;
	}
	if (c < 0x800)
	{
		if (buf_size < 2) return 0;
		buf[0] = (char)(0xc0 + (c >> 6));
		buf[1] = (char)(0x80 + (c & 0x3f));
		return 2;
	}
	if (c >= 0xdc00 && c < 0xe000)
	{
		return 0;
	}
	if (c >= 0xd800 && c < 0xdc00)
	{
		if (buf_size < 4) return 0;
		buf[0] = (char)(0xf0 + (c >> 18));
		buf[1] = (char)(0x80 + ((c >> 12) & 0x3f));
		buf[2] = (char)(0x80 + ((c >> 6) & 0x3f));
		buf[3] = (char)(0x80 + ((c)& 0x3f));
		return 4;
	}
	//else if (c < 0x10000)
	{
		if (buf_size < 3) return 0;
		buf[0] = (char)(0xe0 + (c >> 12));
		buf[1] = (char)(0x80 + ((c >> 6) & 0x3f));
		buf[2] = (char)(0x80 + ((c)& 0x3f));
		return 3;
	}
}

static inline int ImTextCountUtf8BytesFromChar(unsigned int c)
{
	if (c < 0x80) return 1;
	if (c < 0x800) return 2;
	if (c >= 0xdc00 && c < 0xe000) return 0;
	if (c >= 0xd800 && c < 0xdc00) return 4;
	return 3;
}

int ImTextStrToUtf8(char* buf, int buf_size, const ImWchar* in_text, const ImWchar* in_text_end)
{
	char* buf_out = buf;
	const char* buf_end = buf + buf_size;
	while (buf_out < buf_end - 1 && (!in_text_end || in_text < in_text_end) && *in_text)
	{
		unsigned int c = (unsigned int)(*in_text++);
		if (c < 0x80)
			*buf_out++ = (char)c;
		else
			buf_out += ImTextCharToUtf8(buf_out, (int)(buf_end - buf_out - 1), c);
	}
	*buf_out = 0;
	return (int)(buf_out - buf);
}

int ImTextCountUtf8BytesFromStr(const ImWchar* in_text, const ImWchar* in_text_end)
{
	int bytes_count = 0;
	while ((!in_text_end || in_text < in_text_end) && *in_text)
	{
		unsigned int c = (unsigned int)(*in_text++);
		if (c < 0x80)
			bytes_count++;
		else
			bytes_count += ImTextCountUtf8BytesFromChar(c);
	}
	return bytes_count;
}

ImVec4 ImGui::ColorConvertU32ToFloat4(uint in)
{
	float s = 1.0f / 255.0f;
	return ImVec4((in & 0xFF) * s, ((in >> 8) & 0xFF) * s, ((in >> 16) & 0xFF) * s, (in >> 24) * s);
}

uint ImGui::ColorConvertFloat4ToU32(const ImVec4& in)
{
	uint out;
	out = ((uint)IM_F32_TO_INT8(ImSaturate(in.x)));
	out |= ((uint)IM_F32_TO_INT8(ImSaturate(in.y))) << 8;
	out |= ((uint)IM_F32_TO_INT8(ImSaturate(in.z))) << 16;
	out |= ((uint)IM_F32_TO_INT8(ImSaturate(in.w))) << 24;
	return out;
}

// Convert rgb floats ([0-1],[0-1],[0-1]) to hsv floats ([0-1],[0-1],[0-1]), from Foley & van Dam p592
// Optimized http://lolengine.net/blog/2013/01/13/fast-rgb-to-hsv
void ImGui::ColorConvertRGBtoHSV(float r, float g, float b, float& out_h, float& out_s, float& out_v)
{
	float K = 0.f;
	if (g < b)
	{
		const float tmp = g; g = b; b = tmp;
		K = -1.f;
	}
	if (r < g)
	{
		const float tmp = r; r = g; g = tmp;
		K = -2.f / 6.f - K;
	}

	const float chroma = r - (g < b ? g : b);
	out_h = fabsf(K + (g - b) / (6.f * chroma + 1e-20f));
	out_s = chroma / (r + 1e-20f);
	out_v = r;
}

// Convert hsv floats ([0-1],[0-1],[0-1]) to rgb floats ([0-1],[0-1],[0-1]), from Foley & van Dam p593
// also http://en.wikipedia.org/wiki/HSL_and_HSV
void ImGui::ColorConvertHSVtoRGB(float h, float s, float v, float& out_r, float& out_g, float& out_b)
{
	if (s == 0.0f)
	{
		// gray
		out_r = out_g = out_b = v;
		return;
	}

	h = fmodf(h, 1.0f) / (60.0f / 360.0f);
	int   i = (int)h;
	float f = h - (float)i;
	float p = v * (1.0f - s);
	float q = v * (1.0f - s * f);
	float t = v * (1.0f - s * (1.0f - f));

	switch (i)
	{
	case 0: out_r = v; out_g = t; out_b = p; break;
	case 1: out_r = q; out_g = v; out_b = p; break;
	case 2: out_r = p; out_g = v; out_b = t; break;
	case 3: out_r = p; out_g = q; out_b = v; break;
	case 4: out_r = t; out_g = p; out_b = v; break;
	case 5: default: out_r = v; out_g = p; out_b = q; break;
	}
}

// Load file content into memory
// Memory allocated with ImGui::MemAlloc(), must be freed by user using ImGui::MemFree()
void* ImLoadFileToMemory(const char* filename, const char* file_open_mode, int* out_file_size, int padding_bytes)
{
	IM_ASSERT(filename && file_open_mode);
	if (out_file_size)
		*out_file_size = 0;

	FILE* f;
	if ((f = fopen(filename, file_open_mode)) == NULL)
		return NULL;

	long file_size_signed;
	if (fseek(f, 0, SEEK_END) || (file_size_signed = ftell(f)) == -1 || fseek(f, 0, SEEK_SET))
	{
		fclose(f);
		return NULL;
	}

	int file_size = (int)file_size_signed;
	void* file_data = ImGui::MemAlloc(file_size + padding_bytes);
	if (file_data == NULL)
	{
		fclose(f);
		return NULL;
	}
	if (fread(file_data, 1, (size_t)file_size, f) != (size_t)file_size)
	{
		fclose(f);
		ImGui::MemFree(file_data);
		return NULL;
	}
	if (padding_bytes > 0)
		memset((void *)(((char*)file_data) + file_size), 0, padding_bytes);

	fclose(f);
	if (out_file_size)
		*out_file_size = file_size;

	return file_data;
}