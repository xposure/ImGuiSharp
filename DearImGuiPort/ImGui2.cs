using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGui
{
    internal partial class ImGui
    {
        static bool ImIsPointInTriangle(ImVec2 p, ImVec2 a, ImVec2 b, ImVec2 c)
        {
            bool b1 = ((p.x - b.x) * (a.y - b.y) - (p.y - b.y) * (a.x - b.x)) < 0.0f;
            bool b2 = ((p.x - c.x) * (b.y - c.y) - (p.y - c.y) * (b.x - c.x)) < 0.0f;
            bool b3 = ((p.x - a.x) * (c.y - a.y) - (p.y - a.y) * (c.x - a.x)) < 0.0f;
            return ((b1 == b2) && (b2 == b3));
        }

        // Convert rgb floats ([0-1],[0-1],[0-1]) to hsv floats ([0-1],[0-1],[0-1]), from Foley & van Dam p592
        // Optimized http://lolengine.net/blog/2013/01/13/fast-rgb-to-hsv
        static void ColorConvertRGBtoHSV(float r, float g, float b, out float out_h, out float out_s, out float out_v)
        {
            float K = 0;
            if (g < b)
            {
                float tmp = g; g = b; b = tmp;
                K = -1;
            }
            if (r < g)
            {
                float tmp = r; r = g; g = tmp;
                K = -2 / 6 - K;
            }

            float chroma = r - (g < b ? g : b);
            out_h = fabsf(K + (g - b) / (6 * chroma + 1e-20f));
            out_s = chroma / (r + 1e-20f);
            out_v = r;
        }


        void RenderCheckMark(ImVec2 pos, uint col)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            ImVec2 a, b, c;
            float start_x = (float)(int)(g.FontSize * 0.307f + 0.5f);
            float rem_third = (float)(int)((g.FontSize - start_x) / 3.0f);
            a.x = pos.x + 0.5f + start_x;
            b.x = a.x + rem_third;
            c.x = a.x + rem_third * 3.0f;
            b.y = pos.y - 1.0f + (float)(int)(g.Font.Ascent * (g.FontSize / g.Font.FontSize) + 0.5f) + (float)(int)(g.Font.DisplayOffset.y);
            a.y = b.y - rem_third;
            c.y = b.y - rem_third * 2.0f;

            window.DrawList.PathLineTo(a);
            window.DrawList.PathLineTo(b);
            window.DrawList.PathLineTo(c);
            window.DrawList.PathStroke(col, false);
        }


        static char[] DataTypeFormatString(ImGuiDataType data_type, object data_ptr, string display_format, int buf_size)
        {
            char[] buf = new char[buf_size];
            var val = DataTypeFormatStringEx(data_type, data_ptr, display_format);
            for (var i = 0; i < buf_size && i < val.Length; i++)
                buf[i] = val[i];
            return buf;
        }
        static string DataTypeFormatStringEx(ImGuiDataType data_type, object data_ptr, string display_format)
        {
            if (data_type == ImGuiDataType.ImGuiDataType_Int)
                //ImFormatString(buf, buf_size, display_format, *(int*)data_ptr);
                //return string.Format(display_format, Convert.ToInt32(data_ptr));
                return data_ptr.ToString();
            else if (data_type == ImGuiDataType.ImGuiDataType_Float)
                //ImFormatString(buf, buf_size, display_format, *(float*)data_ptr);
                return string.Format(display_format, Convert.ToSingle(data_ptr));

            System.Diagnostics.Debug.Assert(false);
            return string.Empty;
        }

        static char[] DataTypeFormatString(ImGuiDataType data_type, object data_ptr, int decimal_precision, int buf_size)
        {
            char[] buf = new char[buf_size];
            var val = DataTypeFormatStringEx(data_type, data_ptr, decimal_precision);
            for (var i = 0; i < buf_size && i < val.Length; i++)
                buf[i] = val[i];
            return buf;
        }
        static string DataTypeFormatStringEx(ImGuiDataType data_type, object data_ptr, int decimal_precision)
        {
            if (data_type == ImGuiDataType.ImGuiDataType_Int)
            {
                if (decimal_precision < 0)
                    return data_ptr.ToString();
                //ImFormatString(buf, buf_size, "%d", *(int*)data_ptr);
                else
                    return data_ptr.ToString();
                //ImFormatString(buf, buf_size, "%.*d", decimal_precision, *(int*)data_ptr);
            }
            else if (data_type == ImGuiDataType.ImGuiDataType_Float)
            {
                if (decimal_precision < 1)
                    return string.Format("{0}", Convert.ToSingle(data_ptr));
                //ImFormatString(buf, buf_size, "%f", *(float*)data_ptr);     // Ideally we'd have a minimum decimal precision of 1 to visually denote that it is a float, while hiding non-significant digits?
                else
                    //ImFormatString(buf, buf_size, "%.*f", decimal_precision, *(float*)data_ptr);
                    return string.Format("{0:" + new string('0', decimal_precision) + "}", Convert.ToSingle(data_ptr));
            }

            System.Diagnostics.Debug.Assert(false);
            return string.Empty;
        }

        // Create text input in place of a slider (when CTRL+Clicking on slider)
        bool InputScalarAsWidgetReplacement(ImRect aabb, string label, ImGuiDataType data_type, object data_ptr, uint id, int decimal_precision)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            // Our replacement widget will override the focus ID (registered previously to allow for a TAB focus to happen)
            SetActiveID(g.ScalarAsInputTextId, window);
            SetHoveredID(0);
            FocusableItemUnregister(window);

            var buf = DataTypeFormatString(data_type, data_ptr, decimal_precision, 32); ;
            bool value_changed = InputTextEx(label, buf, buf.Length, aabb.GetSize(), ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal | ImGuiInputTextFlags.ImGuiInputTextFlags_AutoSelectAll);
            if (g.ScalarAsInputTextId == 0)
            {
                // First frame
                System.Diagnostics.Debug.Assert(g.ActiveId == id);    // InputText ID expected to match the Slider ID (else we'd need to store them both, which is also possible)
                g.ScalarAsInputTextId = g.ActiveId;
                SetHoveredID(id);
            }
            else if (g.ActiveId != g.ScalarAsInputTextId)
            {
                // Release
                g.ScalarAsInputTextId = 0;
            }
            //TODO: Expression parsing
            //if (value_changed)
            //    DataTypeApplyOpFromText(buf, State.InputTextState.InitialText.begin(), data_type, data_ptr, null);
            return value_changed;
        }

        // Parse display precision back from the display format string
        //int ParseFormatPrecision(string fmt, int default_precision)
        //{
        //    int precision = default_precision;
        //    while ((fmt = strchr(fmt, '%')) != null)
        //    {
        //        fmt++;
        //        if (fmt[0] == '%') { fmt++; continue; } // Ignore "%%"
        //        while (*fmt >= '0' && *fmt <= '9')
        //            fmt++;
        //        if (*fmt == '.')
        //        {
        //            precision = atoi(fmt + 1);
        //            if (precision < 0 || precision > 10)
        //                precision = default_precision;
        //        }
        //        break;
        //    }
        //    return precision;
        //}

        float RoundScalar(float value, int decimal_precision)
        {
            // Round past decimal precision
            // So when our value is 1.99999 with a precision of 0.001 we'll end up rounding to 2.0
            // FIXME: Investigate better rounding methods
            float[] min_steps = { 1.0f, 0.1f, 0.01f, 0.001f, 0.0001f, 0.00001f, 0.000001f, 0.0000001f, 0.00000001f, 0.000000001f };
            float min_step = (decimal_precision >= 0 && decimal_precision < 10) ? min_steps[decimal_precision] : powf(10.0f, (float)-decimal_precision);
            bool negative = value < 0.0f;
            value = fabsf(value);
            float remainder = fmodf(value, min_step);
            if (remainder <= min_step * 0.5f)
                value -= remainder;
            else
                value += (min_step - remainder);
            return negative ? -value : value;
        }

        bool SliderBehavior(ImRect frame_bb, uint id, ref float v, float v_min, float v_max, float power, int decimal_precision, ImGuiSliderFlags flags = 0)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();
            ImGuiStyle style = g.Style;

            // Draw frame
            RenderFrame(frame_bb.Min, frame_bb.Max, GetColorU32(ImGuiCol.ImGuiCol_FrameBg), true, style.FrameRounding);

            bool is_non_linear = fabsf(power - 1.0f) > 0.0001f;
            bool is_horizontal = (flags & ImGuiSliderFlags.ImGuiSliderFlags_Vertical) == 0;

            float grab_padding = 2.0f;
            float slider_sz = is_horizontal ? (frame_bb.GetWidth() - grab_padding * 2.0f) : (frame_bb.GetHeight() - grab_padding * 2.0f);
            float grab_sz;
            if (decimal_precision > 0)
                grab_sz = Min(style.GrabMinSize, slider_sz);
            else
                grab_sz = Min(Max(1.0f * (slider_sz / (v_max - v_min + 1.0f)), style.GrabMinSize), slider_sz);  // Integer sliders, if possible have the grab size represent 1 unit
            float slider_usable_sz = slider_sz - grab_sz;
            float slider_usable_pos_min = (is_horizontal ? frame_bb.Min.x : frame_bb.Min.y) + grab_padding + grab_sz * 0.5f;
            float slider_usable_pos_max = (is_horizontal ? frame_bb.Max.x : frame_bb.Max.y) - grab_padding - grab_sz * 0.5f;

            // For logarithmic sliders that cross over sign boundary we want the exponential increase to be symmetric around 0.0f
            float linear_zero_pos = 0.0f;   // 0.0.1.0f
            if (v_min * v_max < 0.0f)
            {
                // Different sign
                float linear_dist_min_to_0 = powf(fabsf(0.0f - v_min), 1.0f / power);
                float linear_dist_max_to_0 = powf(fabsf(v_max - 0.0f), 1.0f / power);
                linear_zero_pos = linear_dist_min_to_0 / (linear_dist_min_to_0 + linear_dist_max_to_0);
            }
            else
            {
                // Same sign
                linear_zero_pos = v_min < 0.0f ? 1.0f : 0.0f;
            }

            // Process clicking on the slider
            bool value_changed = false;
            if (g.ActiveId == id)
            {
                if (g.IO.MouseDown[0])
                {
                    float mouse_abs_pos = is_horizontal ? g.IO.MousePos.x : g.IO.MousePos.y;
                    float normalized_pos = Clamp((mouse_abs_pos - slider_usable_pos_min) / slider_usable_sz, 0.0f, 1.0f);
                    if (!is_horizontal)
                        normalized_pos = 1.0f - normalized_pos;

                    float new_value;
                    if (is_non_linear)
                    {
                        // Account for logarithmic scale on both sides of the zero
                        if (normalized_pos < linear_zero_pos)
                        {
                            // Negative: rescale to the negative range before powering
                            float a = 1.0f - (normalized_pos / linear_zero_pos);
                            a = powf(a, power);
                            new_value = Lerp(Min(v_max, 0.0f), v_min, a);
                        }
                        else
                        {
                            // Positive: rescale to the positive range before powering
                            float a;
                            if (fabsf(linear_zero_pos - 1.0f) > 1f - 6) //TODO: check 1.e
                                a = (normalized_pos - linear_zero_pos) / (1.0f - linear_zero_pos);
                            else
                                a = normalized_pos;
                            a = powf(a, power);
                            new_value = Lerp(Max(v_min, 0.0f), v_max, a);
                        }
                    }
                    else
                    {
                        // Linear slider
                        new_value = Lerp(v_min, v_max, normalized_pos);
                    }

                    // Round past decimal precision
                    new_value = RoundScalar(new_value, decimal_precision);
                    if (v != new_value)
                    {
                        v = new_value;
                        value_changed = true;
                    }
                }
                else
                {
                    SetActiveID(0);
                }
            }

            // Calculate slider grab positioning
            float grab_t;
            if (is_non_linear)
            {
                float v_clamped = Clamp(v, v_min, v_max);
                if (v_clamped < 0.0f)
                {
                    float f = 1.0f - (v_clamped - v_min) / (Min(0.0f, v_max) - v_min);
                    grab_t = (1.0f - powf(f, 1.0f / power)) * linear_zero_pos;
                }
                else
                {
                    float f = (v_clamped - Max(0.0f, v_min)) / (v_max - Max(0.0f, v_min));
                    grab_t = linear_zero_pos + powf(f, 1.0f / power) * (1.0f - linear_zero_pos);
                }
            }
            else
            {
                // Linear slider
                grab_t = (Clamp(v, v_min, v_max) - v_min) / (v_max - v_min);
            }

            // Draw
            if (!is_horizontal)
                grab_t = 1.0f - grab_t;
            float grab_pos = Lerp(slider_usable_pos_min, slider_usable_pos_max, grab_t);
            ImRect grab_bb;
            if (is_horizontal)
                grab_bb = new ImRect(new ImVec2(grab_pos - grab_sz * 0.5f, frame_bb.Min.y + grab_padding), new ImVec2(grab_pos + grab_sz * 0.5f, frame_bb.Max.y - grab_padding));
            else
                grab_bb = new ImRect(new ImVec2(frame_bb.Min.x + grab_padding, grab_pos - grab_sz * 0.5f), new ImVec2(frame_bb.Max.x - grab_padding, grab_pos + grab_sz * 0.5f));
            window.DrawList.AddRectFilled(grab_bb.Min, grab_bb.Max, GetColorU32(g.ActiveId == id ? ImGuiCol.ImGuiCol_SliderGrabActive : ImGuiCol.ImGuiCol_SliderGrab), style.GrabRounding);

            return value_changed;
        }

        // Use power!=1.0 for logarithmic sliders.
        // Adjust display_format to decorate the value with a prefix or a suffix.
        //   "%.3f"         1.234
        //   "%5.2f secs"   01.23 secs
        //   "Gold: %.0f"   Gold: 1
        bool SliderFloat(string label, ref float v, float v_min, float v_max, string display_format = "{0:0.000}", float power = 1)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);
            float w = CalcItemWidth();

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(w, label_size.y + style.FramePadding.y * 2.0f));
            ImRect total_bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? style.ItemInnerSpacing.x + label_size.x : 0.0f, 0.0f));

            // NB- we don't call ItemSize() yet because we may turn into a text edit box below
            if (!ItemAdd(total_bb, id))
            {
                ItemSize(total_bb, style.FramePadding.y);
                return false;
            }

            bool hovered = IsHovered(frame_bb, id);
            if (hovered)
                SetHoveredID(id);

            if (display_format == null)
                display_format = "%.3f";
            //TODO: Do we need to parse the decimal_precision?
            int decimal_precision = 0;// ParseFormatPrecision(display_format, 3);

            // Tabbing or CTRL-clicking on Slider turns it into an input box
            bool start_text_input = false;
            bool tab_focus_requested = FocusableItemRegister(window, g.ActiveId == id);
            if (tab_focus_requested || (hovered && g.IO.MouseClicked[0]))
            {
                SetActiveID(id, window);
                FocusWindow(window);

                if (tab_focus_requested || g.IO.KeyCtrl)
                {
                    start_text_input = true;
                    g.ScalarAsInputTextId = 0;
                }
            }
            if (start_text_input || (g.ActiveId == id && g.ScalarAsInputTextId == id))
                return InputScalarAsWidgetReplacement(frame_bb, label, ImGuiDataType.ImGuiDataType_Float, v, id, decimal_precision);

            ItemSize(total_bb, style.FramePadding.y);

            // Actual slider behavior + render grab
            bool value_changed = SliderBehavior(frame_bb, id, ref v, v_min, v_max, power, decimal_precision);

            // Display value using user-provided display format so user can add prefix/suffix/decorations to the value.
            var text = string.Format(display_format, v);
            //char value_buf[64];
            //char* value_buf_end = value_buf + ImFormatString(value_buf, IM_ARRAYSIZE(value_buf), display_format, *v);
            RenderTextClipped(frame_bb.Min, frame_bb.Max, text, 0, -1, null, ImGuiAlign.ImGuiAlign_Center | ImGuiAlign.ImGuiAlign_VCenter);

            if (label_size.x > 0.0f)
                RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, frame_bb.Min.y + style.FramePadding.y), label);

            return value_changed;
        }

        bool VSliderFloat(string label, ImVec2 size, ref float v, float v_min, float v_max, string display_format = "{0:0.000}", float power = 1)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + size);
            ImRect bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? style.ItemInnerSpacing.x + label_size.x : 0.0f, 0.0f));

            ItemSize(bb, style.FramePadding.y);
            if (!ItemAdd(frame_bb, id))
                return false;

            bool hovered = IsHovered(frame_bb, id);
            if (hovered)
                SetHoveredID(id);

            if (display_format == null)
                display_format = "%.3f";
            //TODO: Do we need to parse the decimal_precision?
            int decimal_precision = 0;// ParseFormatPrecision(display_format, 3);

            if (hovered && g.IO.MouseClicked[0])
            {
                SetActiveID(id, window);
                FocusWindow(window);
            }

            // Actual slider behavior + render grab
            bool value_changed = SliderBehavior(frame_bb, id, ref v, v_min, v_max, power, decimal_precision, ImGuiSliderFlags.ImGuiSliderFlags_Vertical);

            // Display value using user-provided display format so user can add prefix/suffix/decorations to the value.
            // For the vertical slider we allow centered text to overlap the frame padding
            //char value_buf[64];
            //char* value_buf_end = value_buf + ImFormatString(value_buf, IM_ARRAYSIZE(value_buf), display_format, *v);
            var text = string.Format(display_format, v);
            RenderTextClipped(new ImVec2(frame_bb.Min.x, frame_bb.Min.y + style.FramePadding.y), frame_bb.Max, text, 0, -1, null, ImGuiAlign.ImGuiAlign_Center);
            if (label_size.x > 0.0f)
                RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, frame_bb.Min.y + style.FramePadding.y), label);

            return value_changed;
        }

        bool SliderAngle(string label, ref float v_rad, float v_degrees_min = -360, float v_degrees_max = 360)
        {
            float v_deg = (v_rad) * 360.0f / (2 * PI);
            bool value_changed = SliderFloat(label, ref v_deg, v_degrees_min, v_degrees_max, "{0:0} deg", 1.0f);
            v_rad = v_deg * (2 * PI) / 360.0f;
            return value_changed;
        }

        bool SliderInt(string label, ref int v, int v_min, int v_max, string display_format = "{0:0}")
        {
            if (display_format == null)
                display_format = "{0:0}";
            float v_f = (float)v;
            bool value_changed = SliderFloat(label, ref v_f, (float)v_min, (float)v_max, display_format, 1.0f);
            v = (int)v_f;
            return value_changed;
        }

        bool VSliderInt(string label, ImVec2 size, ref int v, int v_min, int v_max, string display_format = "{0:0}")
        {
            if (display_format == null)
                display_format = "{0:0}";
            float v_f = (float)v;
            bool value_changed = VSliderFloat(label, size, ref v_f, (float)v_min, (float)v_max, display_format, 1.0f);
            v = (int)v_f;
            return value_changed;
        }

        // Add multiple sliders on 1 line for compact edition of multiple components
        bool SliderFloatN(string label, float[] v, int components, float v_min, float v_max, string display_format, float power)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            bool value_changed = false;
            BeginGroup();
            PushID(label);
            PushMultiItemsWidths(components);
            for (int i = 0; i < components; i++)
            {
                PushID(i);
                value_changed |= SliderFloat("##v", ref v[i], v_min, v_max, display_format, power);
                SameLine(0, g.Style.ItemInnerSpacing.x);
                PopID();
                PopItemWidth();
            }
            PopID();

            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();

            return value_changed;
        }

        bool SliderFloat2(string label, float[] v, float v_min, float v_max, string display_format = "{0:0.000}", float power = 1)
        {
            return SliderFloatN(label, v, 2, v_min, v_max, display_format, power);
        }

        bool SliderFloat2(string label, ref ImVec2 _v, float v_min, float v_max, string display_format = "{0:0.000}", float power = 1)
        {
            var v = new float[2];
            v[0] = _v.x;
            v[1] = _v.y;
            var r = SliderFloatN(label, v, 2, v_min, v_max, display_format, power);

            _v.x = v[0];
            _v.y = v[1];
            return r;
        }


        bool SliderFloat3(string label, float[] v, float v_min, float v_max, string display_format = "{0:0.000}", float power = 1)
        {
            return SliderFloatN(label, v, 3, v_min, v_max, display_format, power);
        }

        bool SliderFloat4(string label, float[] v, float v_min, float v_max, string display_format = "{0:0.000}", float power = 1)
        {
            return SliderFloatN(label, v, 4, v_min, v_max, display_format, power);
        }

        bool SliderIntN(string label, int[] v, int components, int v_min, int v_max, string display_format)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            bool value_changed = false;
            BeginGroup();
            PushID(label);
            PushMultiItemsWidths(v.Length);
            for (int i = 0; i < components; i++)
            {
                PushID(i);
                value_changed |= SliderInt("##v", ref v[i], v_min, v_max, display_format);
                SameLine(0, g.Style.ItemInnerSpacing.x);
                PopID();
                PopItemWidth();
            }
            PopID();

            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();

            return value_changed;
        }

        bool SliderInt2(string label, int[] v, int v_min, int v_max, string display_format = "{0:0}")
        {
            return SliderIntN(label, v, 2, v_min, v_max, display_format);
        }

        bool SliderInt3(string label, int[] v, int v_min, int v_max, string display_format = "{0:0}")
        {
            return SliderIntN(label, v, 3, v_min, v_max, display_format);
        }

        bool SliderInt4(string label, int[] v, int v_min, int v_max, string display_format = "{0:0}")
        {
            return SliderIntN(label, v, 4, v_min, v_max, display_format);
        }

        bool DragBehavior(ImRect frame_bb, uint id, ref float v, float v_speed, float v_min, float v_max, int decimal_precision, float power)
        {
            ImGuiState g = State;
            ImGuiStyle style = g.Style;

            // Draw frame
            uint frame_col = GetColorU32(g.ActiveId == id ? ImGuiCol.ImGuiCol_FrameBgActive : g.HoveredId == id ? ImGuiCol.ImGuiCol_FrameBgHovered : ImGuiCol.ImGuiCol_FrameBg);
            RenderFrame(frame_bb.Min, frame_bb.Max, frame_col, true, style.FrameRounding);

            bool value_changed = false;

            // Process clicking on the drag
            if (g.ActiveId == id)
            {
                if (g.IO.MouseDown[0])
                {
                    if (g.ActiveIdIsJustActivated)
                    {
                        // Lock current value on click
                        g.DragCurrentValue = v;
                        g.DragLastMouseDelta = new ImVec2(0f, 0f);
                    }

                    float v_cur = g.DragCurrentValue;
                    ImVec2 mouse_drag_delta = GetMouseDragDelta(0, 1.0f);
                    if (fabsf(mouse_drag_delta.x - g.DragLastMouseDelta.x) > 0.0f)
                    {
                        float speed = v_speed;
                        if (speed == 0.0f && (v_max - v_min) != 0.0f && (v_max - v_min) < float.MaxValue)
                            speed = (v_max - v_min) * g.DragSpeedDefaultRatio;
                        if (g.IO.KeyShift && g.DragSpeedScaleFast >= 0.0f)
                            speed = speed * g.DragSpeedScaleFast;
                        if (g.IO.KeyAlt && g.DragSpeedScaleSlow >= 0.0f)
                            speed = speed * g.DragSpeedScaleSlow;

                        float delta = (mouse_drag_delta.x - g.DragLastMouseDelta.x) * speed;
                        if (fabsf(power - 1.0f) > 0.001f)
                        {
                            // Logarithmic curve on both side of 0.0
                            float v0_abs = v_cur >= 0.0f ? v_cur : -v_cur;
                            float v0_sign = v_cur >= 0.0f ? 1.0f : -1.0f;
                            float v1 = powf(v0_abs, 1.0f / power) + (delta * v0_sign);
                            float v1_abs = v1 >= 0.0f ? v1 : -v1;
                            float v1_sign = v1 >= 0.0f ? 1.0f : -1.0f;          // Crossed sign line
                            v_cur = powf(v1_abs, power) * v0_sign * v1_sign;    // Reapply sign
                        }
                        else
                        {
                            v_cur += delta;
                        }
                        g.DragLastMouseDelta.x = mouse_drag_delta.x;

                        // Clamp
                        if (v_min < v_max)
                            v_cur = Clamp(v_cur, v_min, v_max);
                        g.DragCurrentValue = v_cur;
                    }

                    // Round to user desired precision, then apply
                    v_cur = RoundScalar(v_cur, decimal_precision);
                    if (v != v_cur)
                    {
                        v = v_cur;
                        value_changed = true;
                    }
                }
                else
                {
                    SetActiveID(0);
                }
            }

            return value_changed;
        }

        bool DragFloat(string label, ref float v, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f, string display_format = "{0:0.000}", float power = 1.0f)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);
            float w = CalcItemWidth();

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(w, label_size.y + style.FramePadding.y * 2.0f));
            ImRect inner_bb = new ImRect(frame_bb.Min + style.FramePadding, frame_bb.Max - style.FramePadding);
            ImRect total_bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? style.ItemInnerSpacing.x + label_size.x : 0.0f, 0.0f));

            // NB- we don't call ItemSize() yet because we may turn into a text edit box below
            if (!ItemAdd(total_bb, id))
            {
                ItemSize(total_bb, style.FramePadding.y);
                return false;
            }

            bool hovered = IsHovered(frame_bb, id);
            if (hovered)
                SetHoveredID(id);

            if (display_format == null)
                display_format = "{0:0.000}";
            //TODO: Do we need to parse the decimal_precision?
            int decimal_precision = 0;// ParseFormatPrecision(display_format, 3);

            // Tabbing or CTRL-clicking on Drag turns it into an input box
            bool start_text_input = false;
            bool tab_focus_requested = FocusableItemRegister(window, g.ActiveId == id);
            if (tab_focus_requested || (hovered && (g.IO.MouseClicked[0] | g.IO.MouseDoubleClicked[0])))
            {
                SetActiveID(id, window);
                FocusWindow(window);

                if (tab_focus_requested || g.IO.KeyCtrl || g.IO.MouseDoubleClicked[0])
                {
                    start_text_input = true;
                    g.ScalarAsInputTextId = 0;
                }
            }
            if (start_text_input || (g.ActiveId == id && g.ScalarAsInputTextId == id))
                return InputScalarAsWidgetReplacement(frame_bb, label, ImGuiDataType.ImGuiDataType_Float, v, id, decimal_precision);

            // Actual drag behavior
            ItemSize(total_bb, style.FramePadding.y);
            bool value_changed = DragBehavior(frame_bb, id, ref v, v_speed, v_min, v_max, decimal_precision, power);

            // Display value using user-provided display format so user can add prefix/suffix/decorations to the value.
            //char value_buf[64];
            //char* value_buf_end = value_buf + ImFormatString(value_buf, IM_ARRAYSIZE(value_buf), display_format, v);
            string text = string.Format(display_format, v);
            RenderTextClipped(frame_bb.Min, frame_bb.Max, text, 0, -1, null, ImGuiAlign.ImGuiAlign_Center | ImGuiAlign.ImGuiAlign_VCenter);

            if (label_size.x > 0.0f)
                RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, inner_bb.Min.y), label);

            return value_changed;
        }

        bool DragFloatN(string label, float[] v, int components, float v_speed, float v_min, float v_max, string display_format, float power)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            bool value_changed = false;
            BeginGroup();
            PushID(label);
            PushMultiItemsWidths(components);
            for (int i = 0; i < components; i++)
            {
                PushID(i);
                value_changed |= DragFloat("##v", ref v[i], v_speed, v_min, v_max, display_format, power);
                SameLine(0, g.Style.ItemInnerSpacing.x);
                PopID();
                PopItemWidth();
            }
            PopID();

            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();

            return value_changed;
        }

        bool DragFloat2(string label, float[] v, float v_speed = 1, float v_min = 0, float v_max = 0, string display_format = "{0:0.000}", float power = 1)
        {
            return DragFloatN(label, v, 2, v_speed, v_min, v_max, display_format, power);
        }

        bool DragFloat3(string label, float[] v, float v_speed = 1, float v_min = 0, float v_max = 0, string display_format = "{0:0.000}", float power = 1)
        {
            return DragFloatN(label, v, 3, v_speed, v_min, v_max, display_format, power);
        }

        bool DragFloat4(string label, float[] v, float v_speed = 1, float v_min = 0, float v_max = 0, string display_format = "{0:0.000}", float power = 1)
        {
            return DragFloatN(label, v, 4, v_speed, v_min, v_max, display_format, power);
        }

        bool DragFloatRange2(string label, ref float v_current_min, ref float v_current_max, float v_speed = 1, float v_min = 0, float v_max = 0, string display_format = "{0:0.000}", string display_format_max = null, float power = 1)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            PushID(label);
            BeginGroup();
            PushMultiItemsWidths(2);

            bool value_changed = DragFloat("##min", ref v_current_min, v_speed, (v_min >= v_max) ? -float.MaxValue : v_min, (v_min >= v_max) ? v_current_max : Min(v_max, v_current_max), display_format, power);
            PopItemWidth();
            SameLine(0, g.Style.ItemInnerSpacing.x);
            value_changed |= DragFloat("##max", ref v_current_max, v_speed, (v_min >= v_max) ? v_current_min : Max(v_min, v_current_min), (v_min >= v_max) ? float.MaxValue : v_max, display_format_max != null ? display_format_max : display_format, power);
            PopItemWidth();
            SameLine(0, g.Style.ItemInnerSpacing.x);

            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();
            PopID();

            return value_changed;
        }

        // NB: v_speed is float to allow adjusting the drag speed with more precision
        bool DragInt(string label, ref int v, float v_speed = 1, int v_min = 0, int v_max = 0, string display_format = "{0:0}")
        {
            if (display_format == null)
                display_format = "%.0f";
            float v_f = (float)v;
            bool value_changed = DragFloat(label, ref v_f, v_speed, (float)v_min, (float)v_max, display_format);
            v = (int)v_f;
            return value_changed;
        }

        bool DragIntN(string label, int[] v, int components, float v_speed = 1, int v_min = 0, int v_max = 0, string display_format = "{0:0}")
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            bool value_changed = false;
            BeginGroup();
            PushID(label);
            PushMultiItemsWidths(components);
            for (int i = 0; i < components; i++)
            {
                PushID(i);
                value_changed |= DragInt("##v", ref v[i], v_speed, v_min, v_max, display_format);
                SameLine(0, g.Style.ItemInnerSpacing.x);
                PopID();
                PopItemWidth();
            }
            PopID();

            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();

            return value_changed;
        }

        bool DragInt2(string label, int[] v, float v_speed = 1, int v_min = 0, int v_max = 0, string display_format = "{0:0}")
        {
            return DragIntN(label, v, 2, v_speed, v_min, v_max, display_format);
        }

        bool DragInt3(string label, int[] v, float v_speed = 1, int v_min = 0, int v_max = 0, string display_format = "{0:0}")
        {
            return DragIntN(label, v, 3, v_speed, v_min, v_max, display_format);
        }

        bool DragInt4(string label, int[] v, float v_speed = 1, int v_min = 0, int v_max = 0, string display_format = "{0:0}")
        {
            return DragIntN(label, v, 4, v_speed, v_min, v_max, display_format);
        }

        bool DragIntRange2(string label, ref int v_current_min, ref int v_current_max, float v_speed = 1, int v_min = 0, int v_max = 0, string display_format = "{0:0}", string display_format_max = null)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            PushID(label);
            BeginGroup();
            PushMultiItemsWidths(2);

            bool value_changed = DragInt("##min", ref v_current_min, v_speed, (v_min >= v_max) ? int.MinValue : v_min, (v_min >= v_max) ? v_current_max : Min(v_max, v_current_max), display_format);
            PopItemWidth();
            SameLine(0, g.Style.ItemInnerSpacing.x);
            value_changed |= DragInt("##max", ref v_current_max, v_speed, (v_min >= v_max) ? v_current_min : Max(v_min, v_current_min), (v_min >= v_max) ? int.MaxValue : v_max, display_format_max != null ? display_format_max : display_format);
            PopItemWidth();
            SameLine(0, g.Style.ItemInnerSpacing.x);

            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();
            PopID();

            return value_changed;
        }

        //void PlotEx(ImGuiPlotType plot_type, char* label, float (*values_getter)(void* data, int idx), void* data, int values_count, int values_offset, char* overlay_text, float scale_min, float scale_max, ImVec2 graph_size)
        void PlotEx(ImGuiPlotType plot_type, string label, float[] data, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, ImVec2 graph_size)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            if (graph_size.x == 0.0f)
                graph_size.x = CalcItemWidth();
            if (graph_size.y == 0.0f)
                graph_size.y = label_size.y + (style.FramePadding.y * 2);

            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(graph_size.x, graph_size.y));
            ImRect inner_bb = new ImRect(frame_bb.Min + style.FramePadding, frame_bb.Max - style.FramePadding);
            ImRect total_bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? style.ItemInnerSpacing.x + label_size.x : 0.0f, 0));
            ItemSize(total_bb, style.FramePadding.y);
            if (!ItemAdd(total_bb, null))
                return;

            // Determine scale from values if not specified
            if (scale_min == float.MaxValue || scale_max == float.MaxValue)
            {
                float v_min = float.MaxValue;
                float v_max = -float.MaxValue;
                for (int i = 0; i < values_count; i++)
                {
                    float v = data[i];
                    v_min = Min(v_min, v);
                    v_max = Max(v_max, v);
                }
                if (scale_min == float.MaxValue)
                    scale_min = v_min;
                if (scale_max == float.MaxValue)
                    scale_max = v_max;
            }

            RenderFrame(frame_bb.Min, frame_bb.Max, GetColorU32(ImGuiCol.ImGuiCol_FrameBg), true, style.FrameRounding);

            int res_w = Min((int)graph_size.x, values_count) + ((plot_type == ImGuiPlotType.ImGuiPlotType_Lines) ? -1 : 0);
            int item_count = values_count + ((plot_type == ImGuiPlotType.ImGuiPlotType_Lines) ? -1 : 0);

            // Tooltip on hover
            int v_hovered = -1;
            if (IsHovered(inner_bb, 0))
            {
                float t = Clamp((g.IO.MousePos.x - inner_bb.Min.x) / (inner_bb.Max.x - inner_bb.Min.x), 0.0f, 0.9999f);
                int v_idx = (int)(t * item_count);
                System.Diagnostics.Debug.Assert(v_idx >= 0 && v_idx < values_count);

                float v0 = data[(v_idx + values_offset) % values_count];
                float v1 = data[(v_idx + 1 + values_offset) % values_count];
                if (plot_type == ImGuiPlotType.ImGuiPlotType_Lines)
                    SetTooltip("%d: %8.4g\n%d: %8.4g", v_idx, v0, v_idx + 1, v1);
                else if (plot_type == ImGuiPlotType.ImGuiPlotType_Histogram)
                    SetTooltip("%d: %8.4g", v_idx, v0);
                v_hovered = v_idx;
            }
            {
                float t_step = 1.0f / (float)res_w;

                float v0 = data[(0 + values_offset) % values_count];
                float t0 = 0.0f;
                ImVec2 tp0 = new ImVec2(t0, 1.0f - Saturate((v0 - scale_min) / (scale_max - scale_min)));    // Point in the normalized space of our target rectangle

                uint col_base = GetColorU32((plot_type == ImGuiPlotType.ImGuiPlotType_Lines) ? ImGuiCol.ImGuiCol_PlotLines : ImGuiCol.ImGuiCol_PlotHistogram);
                uint col_hovered = GetColorU32((plot_type == ImGuiPlotType.ImGuiPlotType_Lines) ? ImGuiCol.ImGuiCol_PlotLinesHovered : ImGuiCol.ImGuiCol_PlotHistogramHovered);

                for (int n = 0; n < res_w; n++)
                {
                    float t1 = t0 + t_step;
                    int v1_idx = (int)(t0 * item_count + 0.5f);
                    System.Diagnostics.Debug.Assert(v1_idx >= 0 && v1_idx < values_count);
                    float v1 = data[(v1_idx + values_offset + 1) % values_count];
                    ImVec2 tp1 = new ImVec2(t1, 1.0f - Saturate((v1 - scale_min) / (scale_max - scale_min)));

                    // NB: Draw calls are merged together by the DrawList system. Still, we should render our batch are lower level to save a bit of CPU.
                    ImVec2 pos0 = Lerp(inner_bb.Min, inner_bb.Max, tp0);
                    ImVec2 pos1 = Lerp(inner_bb.Min, inner_bb.Max, (plot_type == ImGuiPlotType.ImGuiPlotType_Lines) ? tp1 : new ImVec2(tp1.x, 1.0f));
                    if (plot_type == ImGuiPlotType.ImGuiPlotType_Lines)
                    {
                        window.DrawList.AddLine(pos0, pos1, v_hovered == v1_idx ? col_hovered : col_base);
                    }
                    else if (plot_type == ImGuiPlotType.ImGuiPlotType_Histogram)
                    {
                        if (pos1.x >= pos0.x + 2.0f)
                            pos1.x -= 1.0f;
                        window.DrawList.AddRectFilled(pos0, pos1, v_hovered == v1_idx ? col_hovered : col_base);
                    }

                    t0 = t1;
                    tp0 = tp1;
                }

                // Text overlay
                if (overlay_text != null)
                    RenderTextClipped(new ImVec2(frame_bb.Min.x, frame_bb.Min.y + style.FramePadding.y), frame_bb.Max, overlay_text, 0, -1, null, ImGuiAlign.ImGuiAlign_Center);

                if (label_size.x > 0.0f)
                    RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, inner_bb.Min.y), label);
            }
        }

        //struct ImGuiPlotArrayGetterData
        //{
        //    float* Values;
        //    int Stride;

        //    ImGuiPlotArrayGetterData(float* values, int stride) { Values = values; Stride = stride; }
        //};

        //float Plot_ArrayGetter(void* data, int idx)
        //{
        //    ImGuiPlotArrayGetterData* plot_data = (ImGuiPlotArrayGetterData*)data;
        //    float v = *(float*)(void*)((unsigned char*)plot_data.Values + (size_t)idx * plot_data.Stride);
        //    return v;
        //}

        void PlotLines(string label, float[] values, int values_count, int values_offset = 0, string overlay_text = null, float scale_min = float.MaxValue, float scale_max = float.MaxValue, ImVec2? _graph_size = null, int stride = sizeof(float))
        {
            var graph_size = _graph_size.HasValue ? _graph_size.Value : ImVec2.Zero;
            //ImGuiPlotArrayGetterData data(values, stride);
            //PlotEx(ImGuiPlotType_Lines, label, &Plot_ArrayGetter, (void*)&data, values_count, values_offset, overlay_text, scale_min, scale_max, graph_size);
            PlotEx(ImGuiPlotType.ImGuiPlotType_Lines, label, values, values_count, values_offset, overlay_text, scale_min, scale_max, graph_size);
        }

        void PlotLines(string label, float[] data, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, ImVec2 graph_size)
        {
            PlotEx(ImGuiPlotType.ImGuiPlotType_Lines, label, data, values_count, values_offset, overlay_text, scale_min, scale_max, graph_size);
        }

        void PlotHistogram(string label, float[] values, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, ImVec2 graph_size, int stride)
        {
            //ImGuiPlotArrayGetterData data(values, stride);
            //PlotEx(ImGuiPlotType_Histogram, label, &Plot_ArrayGetter, (void*)&data, values_count, values_offset, overlay_text, scale_min, scale_max, graph_size);
            PlotEx(ImGuiPlotType.ImGuiPlotType_Histogram, label, values, values_count, values_offset, overlay_text, scale_min, scale_max, graph_size);
        }

        void PlotHistogram(string label, float[] data, int values_count, int values_offset, string overlay_text, float scale_min, float scale_max, ImVec2 graph_size)
        {
            PlotEx(ImGuiPlotType.ImGuiPlotType_Histogram, label, data, values_count, values_offset, overlay_text, scale_min, scale_max, graph_size);
        }

        // size_arg (for each axis) < 0.0f: align to end, 0.0f: auto, > 0.0f: specified size
        void ProgressBar(float fraction, ImVec2 size_arg, string overlay)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;

            ImVec2 pos = window.DC.CursorPos;
            ImRect bb = new ImRect(pos, pos + CalcItemSize(size_arg, CalcItemWidth(), g.FontSize + style.FramePadding.y * 2.0f));
            ItemSize(bb, style.FramePadding.y);
            if (!ItemAdd(bb, null))
                return;

            // Render
            fraction = Saturate(fraction);
            RenderFrame(bb.Min, bb.Max, GetColorU32(ImGuiCol.ImGuiCol_FrameBg), true, style.FrameRounding);
            bb.Reduce(new ImVec2(window.BorderSize, window.BorderSize));
            ImVec2 fill_br = new ImVec2(Lerp(bb.Min.x, bb.Max.x, fraction), bb.Max.y);
            RenderFrame(bb.Min, fill_br, GetColorU32(ImGuiCol.ImGuiCol_PlotHistogram), false, style.FrameRounding);

            // Default displaying the fraction as percentage string, but user can override it
            if (overlay == null)
            {
                overlay = string.Format("{0}%", (int)(fraction * 100 + 0.01f));
                //ImFormatString(overlay_buf, IM_ARRAYSIZE(overlay_buf), "%.0f%%", fraction * 100 + 0.01f);
                //overlay = overlay_buf;
            }

            ImVec2 overlay_size = CalcTextSize(overlay, 0, -1);
            if (overlay_size.x > 0.0f)
                RenderTextClipped(new ImVec2(Clamp(fill_br.x + style.ItemSpacing.x, bb.Min.x, bb.Max.x - overlay_size.x - style.ItemInnerSpacing.x), bb.Min.y), bb.Max, overlay, 0, -1, overlay_size, ImGuiAlign.ImGuiAlign_Left | ImGuiAlign.ImGuiAlign_VCenter, bb.Min, bb.Max);
        }

        //bool InputText(char* label, char* buf, size_t buf_size, ImGuiInputTextFlags flags, ImGuiTextEditCallback callback, void* user_data)
        //{
        //    System.Diagnostics.Debug.Assert(!(flags & ImGuiInputTextFlags.ImGuiInputTextFlags_Multiline)); // call InputTextMultiline()
        //    bool ret = InputTextEx(label, buf, (int)buf_size, new ImVec2(0, 0), flags, callback, user_data);
        //    return ret;
        //}

        bool InputTextMultiline(string label, char[] buf, int buf_size, ImVec2 size, ImGuiInputTextFlags flags = 0, ImGuiTextEditCallback callback = null, object user_data = null)
        {
            bool ret = InputTextEx(label, buf, (int)buf_size, size, flags | ImGuiInputTextFlags.ImGuiInputTextFlags_Multiline, callback, user_data);
            return ret;
        }

        // NB: scalar_format here must be a simple "%xx" format string with no prefix/suffix (unlike the Drag/Slider functions "display_format" argument)
        //bool InputScalarEx(string label, ImGuiDataType data_type, object data_ptr, object step_ptr, object step_fast_ptr, string scalar_format, ImGuiInputTextFlags extra_flags)
        //{
        //    ImGuiWindow window = GetCurrentWindow();
        //    if (window.SkipItems)
        //        return false;

        //    ImGuiState g = State;
        //    ImGuiStyle style = g.Style;
        //    ImVec2 label_size = CalcTextSize(label, 0, -1, true);

        //    BeginGroup();
        //    PushID(label);
        //    ImVec2 button_sz = new ImVec2(g.FontSize, g.FontSize) + style.FramePadding * 2.0f;
        //    //TODO: step ptr
        //    //if (step_ptr)
        //    //    PushItemWidth(Max(1.0f, CalcItemWidth() - (button_sz.x + style.ItemInnerSpacing.x) * 2));

        //    char[] buf = DataTypeFormatString(data_type, data_ptr, scalar_format, 64);

        //    bool value_changed = false;
        //    if ((extra_flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal) == 0)
        //        extra_flags |= ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal;
        //    extra_flags |= ImGuiInputTextFlags.ImGuiInputTextFlags_AutoSelectAll;
        //    if (InputText("", buf, buf.Length, extra_flags))
        //    {
        //        DataTypeApplyOpFromText(buf, State.InputTextState.InitialText.begin(), data_type, data_ptr, scalar_format);
        //        value_changed = true;
        //    }

        //    //TODO: step buttons
        //    //// Step buttons
        //    //if (step_ptr)
        //    //{
        //    //    PopItemWidth();
        //    //    SameLine(0, style.ItemInnerSpacing.x);
        //    //    if (ButtonEx("-", button_sz, ImGuiButtonFlags.ImGuiButtonFlags_Repeat | ImGuiButtonFlags.ImGuiButtonFlags_DontClosePopups))
        //    //    {
        //    //        DataTypeApplyOp(data_type, '-', data_ptr, g.IO.KeyCtrl && step_fast_ptr ? step_fast_ptr : step_ptr);
        //    //        value_changed = true;
        //    //    }
        //    //    SameLine(0, style.ItemInnerSpacing.x);
        //    //    if (ButtonEx("+", button_sz, ImGuiButtonFlags.ImGuiButtonFlags_Repeat | ImGuiButtonFlags.ImGuiButtonFlags_DontClosePopups))
        //    //    {
        //    //        DataTypeApplyOp(data_type, '+', data_ptr, g.IO.KeyCtrl && step_fast_ptr ? step_fast_ptr : step_ptr);
        //    //        value_changed = true;
        //    //    }
        //    //}
        //    PopID();

        //    if (label_size.x > 0)
        //    {
        //        SameLine(0, style.ItemInnerSpacing.x);
        //        RenderText(new ImVec2(window.DC.CursorPos.x, window.DC.CursorPos.y + style.FramePadding.y), label);
        //        ItemSize(label_size, style.FramePadding.y);
        //    }
        //    EndGroup();

        //    return value_changed;
        //}

        bool InputScalarEx(string label, ImGuiDataType data_type, ref int data_ptr, int? step_ptr, int? step_fast_ptr, string scalar_format, ImGuiInputTextFlags extra_flags)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);

            BeginGroup();
            PushID(label);
            ImVec2 button_sz = new ImVec2(g.FontSize, g.FontSize) + style.FramePadding * 2.0f;
            //TODO: step ptr
            if (step_ptr.HasValue)
                PushItemWidth(Max(1.0f, CalcItemWidth() - (button_sz.x + style.ItemInnerSpacing.x) * 2));

            char[] buf = DataTypeFormatString(data_type, data_ptr, scalar_format, 64);

            bool value_changed = false;
            if ((extra_flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal) == 0)
                extra_flags |= ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal;
            extra_flags |= ImGuiInputTextFlags.ImGuiInputTextFlags_AutoSelectAll;
            if (InputText("", buf, buf.Length, extra_flags))
            {
                //TODO: expression
                //DataTypeApplyOpFromText(buf, State.InputTextState.InitialText.begin(), data_type, data_ptr, scalar_format);
                //value_changed = true;
            }

            //TODO: step buttons
            // Step buttons
            if (step_ptr.HasValue)
            {
                PopItemWidth();
                SameLine(0, style.ItemInnerSpacing.x);
                if (ButtonEx("-", button_sz, ImGuiButtonFlags.ImGuiButtonFlags_Repeat | ImGuiButtonFlags.ImGuiButtonFlags_DontClosePopups))
                {
                    //TODO: expression
                    //DataTypeApplyOp(data_type, '-', data_ptr, g.IO.KeyCtrl && step_fast_ptr.HasValue ? step_fast_ptr : step_ptr);
                    //value_changed = true;
                }
                SameLine(0, style.ItemInnerSpacing.x);
                if (ButtonEx("+", button_sz, ImGuiButtonFlags.ImGuiButtonFlags_Repeat | ImGuiButtonFlags.ImGuiButtonFlags_DontClosePopups))
                {
                    //TODO: expression
                    //DataTypeApplyOp(data_type, '+', data_ptr, g.IO.KeyCtrl && step_fast_ptr.HasValue ? step_fast_ptr : step_ptr);
                    //value_changed = true;
                }
            }
            PopID();

            if (label_size.x > 0)
            {
                SameLine(0, style.ItemInnerSpacing.x);
                RenderText(new ImVec2(window.DC.CursorPos.x, window.DC.CursorPos.y + style.FramePadding.y), label);
                ItemSize(label_size, style.FramePadding.y);
            }
            EndGroup();

            return value_changed;
        }

        bool InputScalarEx(string label, ImGuiDataType data_type, ref float data_ptr, float? step_ptr, float? step_fast_ptr, string scalar_format, ImGuiInputTextFlags extra_flags)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);

            BeginGroup();
            PushID(label);
            ImVec2 button_sz = new ImVec2(g.FontSize, g.FontSize) + style.FramePadding * 2.0f;
            //TODO: step ptr
            if (step_ptr.HasValue)
                PushItemWidth(Max(1.0f, CalcItemWidth() - (button_sz.x + style.ItemInnerSpacing.x) * 2));

            char[] buf = DataTypeFormatString(data_type, data_ptr, scalar_format, 64);

            bool value_changed = false;
            if ((extra_flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal) == 0)
                extra_flags |= ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal;
            extra_flags |= ImGuiInputTextFlags.ImGuiInputTextFlags_AutoSelectAll;
            if (InputText("", buf, buf.Length, extra_flags))
            {
                //TODO: expression
                //DataTypeApplyOpFromText(buf, State.InputTextState.InitialText.begin(), data_type, data_ptr, scalar_format);
                //value_changed = true;
            }

            //TODO: step buttons
            // Step buttons
            if (step_ptr.HasValue)
            {
                PopItemWidth();
                SameLine(0, style.ItemInnerSpacing.x);
                if (ButtonEx("-", button_sz, ImGuiButtonFlags.ImGuiButtonFlags_Repeat | ImGuiButtonFlags.ImGuiButtonFlags_DontClosePopups))
                {
                    //TODO: expression
                    //DataTypeApplyOp(data_type, '-', data_ptr, g.IO.KeyCtrl && step_fast_ptr.HasValue ? step_fast_ptr : step_ptr);
                    //value_changed = true;
                }
                SameLine(0, style.ItemInnerSpacing.x);
                if (ButtonEx("+", button_sz, ImGuiButtonFlags.ImGuiButtonFlags_Repeat | ImGuiButtonFlags.ImGuiButtonFlags_DontClosePopups))
                {
                    //TODO: expression
                    //DataTypeApplyOp(data_type, '+', data_ptr, g.IO.KeyCtrl && step_fast_ptr.HasValue ? step_fast_ptr : step_ptr);
                    //value_changed = true;
                }
            }
            PopID();

            if (label_size.x > 0)
            {
                SameLine(0, style.ItemInnerSpacing.x);
                RenderText(new ImVec2(window.DC.CursorPos.x, window.DC.CursorPos.y + style.FramePadding.y), label);
                ItemSize(label_size, style.FramePadding.y);
            }
            EndGroup();

            return value_changed;
        }


        bool InputFloat(string label, ref float v, float step = 0, float step_fast = 0, int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0)
        {
            string display_format = "{0:0.00}";//TODO: hard coded
            //char display_format[16];
            if (decimal_precision < 1)
                //    strcpy(display_format, "%f");      // Ideally we'd have a minimum decimal precision of 1 to visually denote that this is a float, while hiding non-significant digits? %f doesn't have a minimum of 1
                display_format = "{0}";
            else
                //    ImFormatString(display_format, 16, "%%.%df", decimal_precision);
                display_format = "{0:0." + new string('0', decimal_precision) + "}";

            return InputScalarEx(label, ImGuiDataType.ImGuiDataType_Float, ref v, (step > 0.0f ? (float?)step : null), (step_fast > 0.0f ? (float?)step_fast : null), display_format, extra_flags);
        }

        bool InputInt(string label, ref int v, int step = 1, int step_fast = 100, ImGuiInputTextFlags extra_flags = 0)
        {
            // Hexadecimal input provided as a convenience but the flag name is awkward. Typically you'd use InputText() to parse your own data, if you want to handle prefixes.
            string scalar_format = (extra_flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal) != 0 ? "{0:X8}" : "{0}";
            //return InputScalarEx(label, ImGuiDataType.ImGuiDataType_Int, (void*)v, (void*)(step > 0.0f ? &step : null), (void*)(step_fast > 0.0f ? &step_fast : null), scalar_format, extra_flags);
            return InputScalarEx(label, ImGuiDataType.ImGuiDataType_Int, ref v, (step > 0.0f ? (int?)step : null), (step_fast > 0.0f ? (int?)step_fast : null), scalar_format, extra_flags);
        }

        bool InputFloatN(string label, float[] v, int components, int decimal_precision, ImGuiInputTextFlags extra_flags)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            bool value_changed = false;
            BeginGroup();
            PushID(label);
            PushMultiItemsWidths(components);
            for (int i = 0; i < components; i++)
            {
                PushID(i);
                value_changed |= InputFloat("##v", ref v[i], 0, 0, decimal_precision, extra_flags);
                SameLine(0, g.Style.ItemInnerSpacing.x);
                PopID();
                PopItemWidth();
            }
            PopID();

            window.DC.CurrentLineTextBaseOffset = Max(window.DC.CurrentLineTextBaseOffset, g.Style.FramePadding.y);
            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();

            return value_changed;
        }

        bool InputFloat2(string label, float[] v, int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0)
        {
            return InputFloatN(label, v, 2, decimal_precision, extra_flags);
        }

        bool InputFloat3(string label, float[] v, int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0)
        {
            return InputFloatN(label, v, 3, decimal_precision, extra_flags);
        }

        bool InputFloat4(string label, float[] v, int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0)
        {
            return InputFloatN(label, v, 4, decimal_precision, extra_flags);
        }

        bool InputIntN(string label, int[] v, int components, ImGuiInputTextFlags extra_flags)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            bool value_changed = false;
            BeginGroup();
            PushID(label);
            PushMultiItemsWidths(components);
            for (int i = 0; i < components; i++)
            {
                PushID(i);
                value_changed |= InputInt("##v", ref v[i], 0, 0, extra_flags);
                SameLine(0, g.Style.ItemInnerSpacing.x);
                PopID();
                PopItemWidth();
            }
            PopID();

            window.DC.CurrentLineTextBaseOffset = Max(window.DC.CurrentLineTextBaseOffset, g.Style.FramePadding.y);
            TextUnformatted(label, FindRenderedTextEnd(label));
            EndGroup();

            return value_changed;
        }

        bool InputInt2(string label, int[] v, ImGuiInputTextFlags extra_flags = 0)
        {
            return InputIntN(label, v, 2, extra_flags);
        }

        bool InputInt3(string label, int[] v, ImGuiInputTextFlags extra_flags = 0)
        {
            return InputIntN(label, v, 3, extra_flags);
        }

        bool InputInt4(string label, int[] v, ImGuiInputTextFlags extra_flags = 0)
        {
            return InputIntN(label, v, 4, extra_flags);
        }


        bool MenuItem(string label, string shortcut, bool selected, bool enabled = true)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImVec2 pos = window.DC.CursorPos;
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImVec2 shortcut_size = shortcut != null ? CalcTextSize(shortcut, 0, -1) : new ImVec2(0.0f, 0.0f);
            float w = window.MenuColumns.DeclColumns(label_size.x, shortcut_size.x, (float)(int)(g.FontSize * 1.20f)); // Feedback for next frame
            float extra_w = Max(0.0f, GetContentRegionAvail().x - w);

            bool pressed = SelectableEx(label, false, ImGuiSelectableFlags.ImGuiSelectableFlags_MenuItem | ImGuiSelectableFlags.ImGuiSelectableFlags_DrawFillAvailWidth | (!enabled ? ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled : 0), new ImVec2(w, 0.0f));
            if (shortcut_size.x > 0.0f)
            {
                PushStyleColor(ImGuiCol.ImGuiCol_Text, g.Style.Colors[(int)ImGuiCol.ImGuiCol_TextDisabled]);
                RenderText(pos + new ImVec2(window.MenuColumns.Pos[1] + extra_w, 0.0f), shortcut, 0, -1, false);
                PopStyleColor();
            }

            if (selected)
                RenderCheckMark(pos + new ImVec2(window.MenuColumns.Pos[2] + extra_w + g.FontSize * 0.20f, 0.0f), GetColorU32(ImGuiCol.ImGuiCol_Text));

            return pressed;
        }

        bool MenuItem(string label, string shortcut, ref bool p_selected, bool enabled = true)
        {
            if (MenuItem(label, shortcut, p_selected ? p_selected : false, enabled))
            {
                //if (p_selected)
                p_selected = !p_selected;
                return true;
            }
            return false;
        }

        bool BeginMainMenuBar()
        {
            ImGuiState g = State;
            SetNextWindowPos(new ImVec2(0.0f, 0.0f));
            SetNextWindowSize(new ImVec2(g.IO.DisplaySize.x, g.FontBaseSize + g.Style.FramePadding.y * 2.0f));
            PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_WindowRounding, 0.0f);
            PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_WindowMinSize, new ImVec2(0, 0));
            var throwAway = true;
            if (!Begin("##MainMenuBar", ref throwAway, ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags.ImGuiWindowFlags_NoResize | ImGuiWindowFlags.ImGuiWindowFlags_NoMove | ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings | ImGuiWindowFlags.ImGuiWindowFlags_MenuBar)
        || !BeginMenuBar())
            {
                End();
                PopStyleVar(2);
                return false;
            }
            g.CurrentWindow.DC.MenuBarOffsetX += g.Style.DisplaySafeAreaPadding.x;
            return true;
        }

        void EndMainMenuBar()
        {
            EndMenuBar();
            End();
            PopStyleVar(2);
        }

        bool BeginMenuBar()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_MenuBar) == 0)
                return false;

            System.Diagnostics.Debug.Assert(!window.DC.MenuBarAppending);
            BeginGroup(); // Save position
            PushID("##menubar");
            ImRect rect = window.MenuBarRect();
            PushClipRect(new ImVec2(rect.Min.x + 0.5f, rect.Min.y - 0.5f + window.BorderSize), new ImVec2(rect.Max.x + 0.5f, rect.Max.y - 0.5f), false);
            window.DC.CursorPos = new ImVec2(rect.Min.x + window.DC.MenuBarOffsetX, rect.Min.y);// + g.Style.FramePadding.y);
            window.DC.LayoutType = ImGuiLayoutType.ImGuiLayoutType_Horizontal;
            window.DC.MenuBarAppending = true;
            AlignFirstTextHeightToWidgets();
            return true;
        }

        void EndMenuBar()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            System.Diagnostics.Debug.Assert((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_MenuBar) != 0);
            System.Diagnostics.Debug.Assert(window.DC.MenuBarAppending);
            PopClipRect();
            PopID();
            window.DC.MenuBarOffsetX = window.DC.CursorPos.x - window.MenuBarRect().Min.x;
            var r = window.DC.GroupStack.back();
            r.AdvanceCursor = false;
            window.DC.GroupStack.back(r);

            EndGroup();
            window.DC.LayoutType = ImGuiLayoutType.ImGuiLayoutType_Vertical;
            window.DC.MenuBarAppending = false;
        }

        bool BeginMenu(string label, bool enabled = true)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImGuiWindow backed_focused_window = g.FocusedWindow;

            bool pressed;
            bool opened = IsPopupOpen(id);
            bool menuset_opened = (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) == 0 && (g.OpenedPopupStack.Size > g.CurrentPopupStack.Size && g.OpenedPopupStack[g.CurrentPopupStack.Size].ParentMenuSet == window.GetID("##menus"));
            if (menuset_opened)
                g.FocusedWindow = window;

            ImVec2 popup_pos, pos = window.DC.CursorPos;
            if (window.DC.LayoutType == ImGuiLayoutType.ImGuiLayoutType_Horizontal)
            {
                popup_pos = new ImVec2(pos.x - window.WindowPadding.x, pos.y - style.FramePadding.y + window.MenuBarHeight());
                window.DC.CursorPos.x += (float)(int)(style.ItemSpacing.x * 0.5f);
                PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_ItemSpacing, style.ItemSpacing * 2.0f);
                float w = label_size.x;
                pressed = SelectableEx(label, opened, ImGuiSelectableFlags.ImGuiSelectableFlags_Menu | ImGuiSelectableFlags.ImGuiSelectableFlags_DontClosePopups | (!enabled ? ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled : 0), new ImVec2(w, 0.0f));
                PopStyleVar();
                SameLine();
                window.DC.CursorPos.x += (float)(int)(style.ItemSpacing.x * 0.5f);
            }
            else
            {
                popup_pos = new ImVec2(pos.x, pos.y - style.WindowPadding.y);
                float w = window.MenuColumns.DeclColumns(label_size.x, 0.0f, (float)(int)(g.FontSize * 1.20f)); // Feedback to next frame
                float extra_w = Max(0.0f, GetContentRegionAvail().x - w);
                pressed = SelectableEx(label, opened, ImGuiSelectableFlags.ImGuiSelectableFlags_Menu | ImGuiSelectableFlags.ImGuiSelectableFlags_DontClosePopups | ImGuiSelectableFlags.ImGuiSelectableFlags_DrawFillAvailWidth | (!enabled ? ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled : 0), new ImVec2(w, 0.0f));
                if (!enabled) PushStyleColor(ImGuiCol.ImGuiCol_Text, g.Style.Colors[(int)ImGuiCol.ImGuiCol_TextDisabled]);
                RenderCollapseTriangle(pos + new ImVec2(window.MenuColumns.Pos[2] + extra_w + g.FontSize * 0.20f, 0.0f), false);
                if (!enabled) PopStyleColor();
            }

            bool hovered = enabled && IsHovered(window.DC.LastItemRect, id);
            if (menuset_opened)
                g.FocusedWindow = backed_focused_window;

            bool want_open = false, want_close = false;
            if ((window.Flags & (ImGuiWindowFlags.ImGuiWindowFlags_Popup | ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu)) != 0)
            {
                // Implement http://bjk5.com/post/44698559168/breaking-down-amazons-mega-dropdown to avoid using timers so menus feel more reactive.
                bool moving_within_opened_triangle = false;
                if (g.HoveredWindow == window && g.OpenedPopupStack.Size > g.CurrentPopupStack.Size && g.OpenedPopupStack[g.CurrentPopupStack.Size].ParentWindow == window)
                {
                    ImGuiWindow next_window = g.OpenedPopupStack[g.CurrentPopupStack.Size].Window;
                    if (window != null)
                    {
                        ImRect next_window_rect = next_window.Rect();
                        ImVec2 ta = g.IO.MousePos - g.IO.MouseDelta;
                        ImVec2 tb = (window.Pos.x < next_window.Pos.x) ? next_window_rect.GetTL() : next_window_rect.GetTR();
                        ImVec2 tc = (window.Pos.x < next_window.Pos.x) ? next_window_rect.GetBL() : next_window_rect.GetBR();
                        float extra = Clamp(fabsf(ta.x - tb.x) * 0.30f, 5.0f, 30.0f); // add a bit of extra slack.
                        ta.x += (window.Pos.x < next_window.Pos.x) ? -0.5f : +0.5f;   // to avoid numerical issues
                        tb.y = ta.y + Max((tb.y - extra) - ta.y, -100.0f);            // triangle is maximum 200 high to limit the slope and the bias toward large sub-menus
                        tc.y = ta.y + Min((tc.y + extra) - ta.y, +100.0f);
                        moving_within_opened_triangle = ImIsPointInTriangle(g.IO.MousePos, ta, tb, tc);
                        //window.DrawList.PushClipRectFullScreen(); window.DrawList.AddTriangleFilled(ta, tb, tc, moving_within_opened_triangle ? 0x80008000 : 0x80000080); window.DrawList.PopClipRect(); // Debug
                    }
                }

                want_close = (opened && !hovered && g.HoveredWindow == window && g.HoveredIdPreviousFrame != 0 && g.HoveredIdPreviousFrame != id && !moving_within_opened_triangle);
                want_open = (!opened && hovered && !moving_within_opened_triangle) || (!opened && hovered && pressed);
            }
            else if (opened && pressed && menuset_opened) // menu-bar: click open menu to close
            {
                want_close = true;
                want_open = opened = false;
            }
            else if (pressed || (hovered && menuset_opened && !opened)) // menu-bar: first click to open, then hover to open others
                want_open = true;

            if (want_close && IsPopupOpen(id))
                ClosePopupToLevel(State.CurrentPopupStack.Size);

            if (!opened && want_open && g.OpenedPopupStack.Size > g.CurrentPopupStack.Size)
            {
                // Don't recycle same menu level in the same frame, first close the other menu and yield for a frame.
                OpenPopup(label);
                return false;
            }

            opened |= want_open;
            if (want_open)
                OpenPopup(label);

            if (opened)
            {
                SetNextWindowPos(popup_pos, ImGuiSetCond.ImGuiSetCond_Always);
                ImGuiWindowFlags flags = ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders | ((window.Flags & (ImGuiWindowFlags.ImGuiWindowFlags_Popup | ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu)) != 0 ? ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu | ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow : ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu);
                opened = BeginPopupEx(label, flags); // opened can be 'false' when the popup is completely clipped (e.g. zero size display)
            }

            return opened;
        }

        void EndMenu()
        {
            EndPopup();
        }




        //// Combo box helper allowing to pass an array of strings.
        //        bool Combo(string label, ref int current_item, string[] items, int items_count, int height_in_items)
        //{
        //    bool value_changed = Combo(label, current_item, Items_ArrayGetter, (void*)items, items_count, height_in_items);
        //    return value_changed;
        //}

        //// Combo box helper allowing to pass all items in a single string.
        //bool Combo(string label, ref int current_item, char* items_separated_by_zeros, int height_in_items)
        //{
        //    int items_count = 0;
        //    char* p = items_separated_by_zeros;       // FIXME-OPT: Avoid computing this, or at least only when combo is open
        //    while (*p)
        //    {
        //        p += strlen(p) + 1;
        //        items_count++;
        //    }
        //    bool value_changed = Combo(label, current_item, Items_SingleStringGetter, (void*)items_separated_by_zeros, items_count, height_in_items);
        //    return value_changed;
        //}

        // Combo box function.
        bool Combo(string label, ref int current_item, string[] items, /*int items_count,*/ int height_in_items = -1)
        //bool Combo(string label, ref int current_item, bool (*items_getter)(void*, int, char**), void* data, int items_count, int height_in_items)
        {
            var items_count = items.Length;
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);
            float w = CalcItemWidth();

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(w, label_size.y + style.FramePadding.y * 2.0f));
            ImRect total_bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? style.ItemInnerSpacing.x + label_size.x : 0.0f, 0.0f));
            ItemSize(total_bb, style.FramePadding.y);
            if (!ItemAdd(total_bb, id))
                return false;

            float arrow_size = (g.FontSize + style.FramePadding.x * 2.0f);
            bool hovered = IsHovered(frame_bb, id);

            ImRect value_bb = new ImRect(frame_bb.Min, frame_bb.Max - new ImVec2(arrow_size, 0.0f));
            RenderFrame(frame_bb.Min, frame_bb.Max, GetColorU32(ImGuiCol.ImGuiCol_FrameBg), true, style.FrameRounding);
            RenderFrame(new ImVec2(frame_bb.Max.x - arrow_size, frame_bb.Min.y), frame_bb.Max, GetColorU32(hovered ? ImGuiCol.ImGuiCol_ButtonHovered : ImGuiCol.ImGuiCol_Button), true, style.FrameRounding); // FIXME-ROUNDING
            RenderCollapseTriangle(new ImVec2(frame_bb.Max.x - arrow_size, frame_bb.Min.y) + style.FramePadding, true);

            if (current_item >= 0 && current_item < items_count)
            {
                string item_text = items[current_item];
                //if (items_getter(data, current_item, &item_text))
                RenderTextClipped(frame_bb.Min + style.FramePadding, value_bb.Max, item_text, 0, -1, null);
            }

            if (label_size.x > 0)
                RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, frame_bb.Min.y + style.FramePadding.y), label);

            bool menu_toggled = false;
            if (hovered)
            {
                SetHoveredID(id);
                if (g.IO.MouseClicked[0])
                {
                    SetActiveID(0);
                    if (IsPopupOpen(id))
                    {
                        ClosePopup(id);
                    }
                    else
                    {
                        FocusWindow(window);
                        OpenPopup(label);
                        menu_toggled = true;
                    }
                }
            }

            bool value_changed = false;
            if (IsPopupOpen(id))
            {
                // Size default to hold ~7 items
                if (height_in_items < 0)
                    height_in_items = 7;

                float popup_height = (label_size.y + style.ItemSpacing.y) * Min(items_count, height_in_items) + (style.FramePadding.y * 3);
                ImRect popup_rect = new ImRect(new ImVec2(frame_bb.Min.x, frame_bb.Max.y), new ImVec2(frame_bb.Max.x, frame_bb.Max.y + popup_height));
                popup_rect.Max.y = Min(popup_rect.Max.y, g.IO.DisplaySize.y - style.DisplaySafeAreaPadding.y); // Adhoc height limit for Combo. Ideally should be handled in Begin() along with other popups size, we want to have the possibility of moving the popup above as well.
                SetNextWindowPos(popup_rect.Min);
                SetNextWindowSize(popup_rect.GetSize());
                PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_WindowPadding, style.FramePadding);

                ImGuiWindowFlags flags = ImGuiWindowFlags.ImGuiWindowFlags_ComboBox | ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) != 0 ? ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders : 0);
                if (BeginPopupEx(label, flags))
                {
                    // Display items
                    Spacing();
                    for (int i = 0; i < items_count; i++)
                    {
                        PushID(i.ToString());
                        bool item_selected = (i == current_item);
                        string item_text = items[i];
                        //if (!items_getter(data, i, &item_text))
                        //    item_text = "*Unknown item*";
                        if (Selectable(item_text, ref item_selected))
                        {
                            SetActiveID(0);
                            value_changed = true;
                            current_item = i;
                        }
                        if (item_selected && menu_toggled)
                            SetScrollHere();
                        PopID();
                    }
                    EndPopup();
                }
                PopStyleVar();
            }
            return value_changed;
        }

        // Tip: pass an empty label (e.g. "##dummy") then you can use the space to draw other text or image.
        // But you need to make sure the ID is unique, e.g. enclose calls in PushID/PopID.
        bool SelectableEx(string label, bool selected, ImGuiSelectableFlags flags, ImVec2 size_arg)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;

            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_SpanAllColumns) != 0 && window.DC.ColumnsCount > 1)
                PopClipRect();

            uint id = window.GetID(label);
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImVec2 size = new ImVec2(size_arg.x != 0.0f ? size_arg.x : label_size.x, size_arg.y != 0.0f ? size_arg.y : label_size.y);
            ImVec2 pos = window.DC.CursorPos;
            pos.y += window.DC.CurrentLineTextBaseOffset;
            ImRect bb = new ImRect(pos, pos + size);
            ItemSize(bb);

            // Fill horizontal space.
            ImVec2 window_padding = window.WindowPadding;
            float max_x = (flags & ImGuiSelectableFlags.ImGuiSelectableFlags_SpanAllColumns) != 0 ? GetWindowContentRegionMax().x : GetContentRegionMax().x;
            float w_draw = Max(label_size.x, window.Pos.x + max_x - window_padding.x - window.DC.CursorPos.x);
            ImVec2 size_draw = new ImVec2((size_arg.x != 0 && (flags & ImGuiSelectableFlags.ImGuiSelectableFlags_DrawFillAvailWidth) == 0) ? size_arg.x : w_draw, size_arg.y != 0.0f ? size_arg.y : size.y);
            ImRect bb_with_spacing = new ImRect(pos, pos + size_draw);
            if (size_arg.x == 0.0f || (flags & ImGuiSelectableFlags.ImGuiSelectableFlags_DrawFillAvailWidth) != 0)
                bb_with_spacing.Max.x += window_padding.x;

            // Selectables are tightly packed together, we extend the box to cover spacing between selectable.
            float spacing_L = (int)(style.ItemSpacing.x * 0.5f);
            float spacing_U = (int)(style.ItemSpacing.y * 0.5f);
            float spacing_R = style.ItemSpacing.x - spacing_L;
            float spacing_D = style.ItemSpacing.y - spacing_U;
            bb_with_spacing.Min.x -= spacing_L;
            bb_with_spacing.Min.y -= spacing_U;
            bb_with_spacing.Max.x += spacing_R;
            bb_with_spacing.Max.y += spacing_D;
            if (!ItemAdd(bb_with_spacing, id))
            {
                if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_SpanAllColumns) != 0 && window.DC.ColumnsCount > 1)
                    PushColumnClipRect();
                return false;
            }

            ImGuiButtonFlags button_flags = 0;
            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_Menu) != 0) button_flags |= ImGuiButtonFlags.ImGuiButtonFlags_PressedOnClick;
            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_MenuItem) != 0) button_flags |= ImGuiButtonFlags.ImGuiButtonFlags_PressedOnClick | ImGuiButtonFlags.ImGuiButtonFlags_PressedOnRelease;
            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled) != 0) button_flags |= ImGuiButtonFlags.ImGuiButtonFlags_Disabled;
            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_AllowDoubleClick) != 0) button_flags |= ImGuiButtonFlags.ImGuiButtonFlags_PressedOnDoubleClick;
            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(bb_with_spacing, id, ref hovered, ref held, button_flags);
            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled) != 0)
                selected = false;

            // Render
            if (hovered.Value || selected)
            {
                uint col = GetColorU32((held.Value && hovered.Value) ? ImGuiCol.ImGuiCol_HeaderActive : hovered.Value ? ImGuiCol.ImGuiCol_HeaderHovered : ImGuiCol.ImGuiCol_Header);
                RenderFrame(bb_with_spacing.Min, bb_with_spacing.Max, col, false, 0.0f);
            }

            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_SpanAllColumns) != 0 && window.DC.ColumnsCount > 1)
            {
                PushColumnClipRect();
                bb_with_spacing.Max.x -= (GetContentRegionMax().x - max_x);
            }

            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled) != 0) PushStyleColor(ImGuiCol.ImGuiCol_Text, g.Style.Colors[(int)ImGuiCol.ImGuiCol_TextDisabled]);
            RenderTextClipped(bb.Min, bb_with_spacing.Max, label, 0, -1, label_size);
            if ((flags & ImGuiSelectableFlags.ImGuiSelectableFlags_Disabled) != 0) PopStyleColor();

            // Automatically close popups
            if (pressed && (flags & ImGuiSelectableFlags.ImGuiSelectableFlags_DontClosePopups) == 0 && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0)
                CloseCurrentPopup();
            return pressed;
        }

        //bool Selectable(string label, ref bool selected, ImGuiSelectableFlags flags = 0, ImVec2? _size_arg = null)
        //{
        //    var size_arg = _size_arg.HasValue ? _size_arg.Value : ImVec2.Zero;
        //    return SelectableEx(label, ref selected, flags, size_arg);
        //}

        bool Selectable(string label, ref bool p_selected, ImGuiSelectableFlags flags = 0, ImVec2? _size_arg = null)
        {
            var size_arg = _size_arg.HasValue ? _size_arg.Value : ImVec2.Zero;
            if (SelectableEx(label, p_selected, flags, size_arg))
            {
                p_selected = !p_selected;
                return true;
            }
            return false;
        }

        // Helper to calculate the size of a listbox and display a label on the right.
        // Tip: To have a list filling the entire window width, PushItemWidth(-1) and pass an empty label "##empty"
        bool ListBoxHeader(string label, ImVec2 size_arg)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiStyle style = GetStyle();
            uint id = GetID(label);
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);

            // Size default to hold ~7 items. Fractional number of items helps seeing that we can scroll down/up without looking at scrollbar.
            ImVec2 size = CalcItemSize(size_arg, CalcItemWidth(), GetTextLineHeightWithSpacing() * 7.4f + style.ItemSpacing.y);
            ImVec2 frame_size = new ImVec2(size.x, Max(size.y, label_size.y));
            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + frame_size);
            ImRect bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? style.ItemInnerSpacing.x + label_size.x : 0.0f, 0.0f));
            window.DC.LastItemRect = bb;

            BeginGroup();
            if (label_size.x > 0)
                RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, frame_bb.Min.y + style.FramePadding.y), label);

            BeginChildFrame(id, frame_bb.GetSize());
            return true;
        }

        bool ListBoxHeader(string label, int items_count, int height_in_items)
        {
            // Size default to hold ~7 items. Fractional number of items helps seeing that we can scroll down/up without looking at scrollbar.
            // However we don't add +0.40f if items_count <= height_in_items. It is slightly dodgy, because it means a dynamic list of items will make the widget resize occasionally when it crosses that size.
            // I am expecting that someone will come and complain about this behavior in a remote future, then we can advise on a better solution.
            if (height_in_items < 0)
                height_in_items = Min(items_count, 7);
            float height_in_items_f = height_in_items < items_count ? (height_in_items + 0.40f) : (height_in_items + 0.00f);

            // We include ItemSpacing.y so that a list sized for the exact number of items doesn't make a scrollbar appears. We could also enforce that by passing a flag to BeginChild().
            ImVec2 size;
            size.x = 0.0f;
            size.y = GetTextLineHeightWithSpacing() * height_in_items_f + GetStyle().ItemSpacing.y;
            return ListBoxHeader(label, size);
        }

        void ListBoxFooter()
        {
            ImGuiWindow parent_window = GetParentWindow();
            ImRect bb = parent_window.DC.LastItemRect;
            ImGuiStyle style = GetStyle();

            EndChildFrame();

            // Redeclare item size so that it includes the label (we have stored the full size in LastItemRect)
            // We call SameLine() to restore DC.CurrentLine* data
            SameLine();
            parent_window.DC.CursorPos = bb.Min;
            ItemSize(bb, style.FramePadding.y);
            EndGroup();
        }

        //bool ListBox(string label, ref int current_item, string[] items, int items_count, int height_items)
        //{
        //    bool value_changed = ListBox(label, current_item, Items_ArrayGetter, (void*)items, items_count, height_items);
        //    return value_changed;
        //}

        bool ListBox(string label, ref int current_item, string[] items, int height_in_items)
        //bool ListBox(char* label, int* current_item, bool (*items_getter)(void*, int, char**), void* data, int items_count, int height_in_items)
        {
            int items_count = items.Length;
            if (!ListBoxHeader(label, items_count, height_in_items))
                return false;

            // Assume all items have even height (= 1 line of text). If you need items of different or variable sizes you can create a custom version of ListBox() in your code without using the clipper.
            bool value_changed = false;
            ImGuiListClipper clipper = new ImGuiListClipper(items_count, GetTextLineHeightWithSpacing());
            for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            {
                bool item_selected = (i == current_item);
                var item_text = items[i];
                //if (!items_getter(data, i, &item_text))
                //    item_text = "*Unknown item*";

                PushID(i);
                if (Selectable(item_text, ref item_selected))
                {
                    current_item = i;
                    value_changed = true;
                }
                PopID();
            }
            clipper.End();
            ListBoxFooter();
            return value_changed;
        }


        void Image(ImTextureID user_texture_id, ImVec2 size, ImVec2 uv0, ImVec2 uv1, ImVec4 tint_col, ImVec4 border_col)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + size);
            if (border_col.w > 0.0f)
                bb.Max += new ImVec2(2, 2);
            ItemSize(bb);
            if (!ItemAdd(bb, null))
                return;

            if (border_col.w > 0.0f)
            {
                window.DrawList.AddRect(bb.Min, bb.Max, GetColorU32(border_col), 0.0f);
                window.DrawList.AddImage(user_texture_id, bb.Min + new ImVec2(1, 1), bb.Max - new ImVec2(1, 1), uv0, uv1, GetColorU32(tint_col));
            }
            else
            {
                window.DrawList.AddImage(user_texture_id, bb.Min, bb.Max, uv0, uv1, GetColorU32(tint_col));
            }
        }

        // frame_padding < 0: uses FramePadding from style (default)
        // frame_padding = 0: no framing
        // frame_padding > 0: set framing size
        // The color used are the button colors.

        bool ImageButton(ImTextureID user_texture_id, ImVec2 size, ImVec2? _uv0 = null, ImVec2? _uv1 = null, int frame_padding = -1, ImVec4? _bg_col = null, ImVec4? _tint_col = null)
        {
            var uv0 = _uv0.HasValue ? _uv0.Value : ImVec2.Zero;
            var uv1 = _uv1.HasValue ? _uv1.Value : ImVec2.One;
            var bg_col = _tint_col.HasValue ? _bg_col.Value : new ImVec4(0, 0, 0, 0);
            var tint_col = _tint_col.HasValue ? _tint_col.Value : new ImVec4(1, 1, 1, 1);

            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;

            // Default to using texture ID as ID. User can still push string/integer prefixes.
            // We could hash the size/uv to create a unique ID but that would prevent the user from animating UV.
            //TODO: PushID((void*)user_texture_id);
            uint id = window.GetID("#image");
            //PopID();

            ImVec2 padding = (frame_padding >= 0) ? new ImVec2((float)frame_padding, (float)frame_padding) : style.FramePadding;
            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + size + padding * 2);
            ImRect image_bb = new ImRect(window.DC.CursorPos + padding, window.DC.CursorPos + padding + size);
            ItemSize(bb);
            if (!ItemAdd(bb, id))
                return false;

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(bb, id, ref hovered, ref held);

            // Render
            uint col = GetColorU32((hovered.Value && held.Value) ? ImGuiCol.ImGuiCol_ButtonActive : hovered.Value ? ImGuiCol.ImGuiCol_ButtonHovered : ImGuiCol.ImGuiCol_Button);
            RenderFrame(bb.Min, bb.Max, col, true, Clamp((float)Min(padding.x, padding.y), 0.0f, style.FrameRounding));
            if (bg_col.w > 0.0f)
                window.DrawList.AddRectFilled(image_bb.Min, image_bb.Max, GetColorU32(bg_col));
            window.DrawList.AddImage(user_texture_id, image_bb.Min, image_bb.Max, uv0, uv1, GetColorU32(tint_col));

            return pressed;
        }

        bool IsPopupOpen(uint id)
        {
            ImGuiState g = State;
            bool opened = g.OpenedPopupStack.Size > g.CurrentPopupStack.Size && g.OpenedPopupStack[g.CurrentPopupStack.Size].PopupID == id;
            return opened;
        }

        // Mark popup as open (toggle toward open state). 
        // Popups are closed when user click outside, or activate a pressable item, or CloseCurrentPopup() is called within a BeginPopup()/EndPopup() block.
        // Popup identifiers are relative to the current ID-stack (so OpenPopup and BeginPopup needs to be at the same level).
        // One open popup per level of the popup hierarchy (NB: when assigning we reset the Window member of ImGuiPopupRef to null)
        void OpenPopupEx(string str_id, bool reopen_existing)
        {
            ImGuiState g = State;
            ImGuiWindow window = g.CurrentWindow;
            uint id = window.GetID(str_id);
            int current_stack_size = g.CurrentPopupStack.Size;
            ImGuiPopupRef popup_ref = new ImGuiPopupRef(id, window, window.GetID("##menus"), g.IO.MousePos); // Tagged as new ref because constructor sets Window to null (we are passing the ParentWindow info here)
            if (g.OpenedPopupStack.Size < current_stack_size + 1)
                g.OpenedPopupStack.push_back(popup_ref);
            else if (reopen_existing || g.OpenedPopupStack[current_stack_size].PopupID != id)
            {
                g.OpenedPopupStack.resize(current_stack_size + 1);
                g.OpenedPopupStack[current_stack_size] = popup_ref;
            }
        }

        void OpenPopup(string str_id)
        {
            OpenPopupEx(str_id, false);
        }

        //void CloseInactivePopups()
        //{
        //    ImGuiState g = State;
        //    if (g.OpenedPopupStack.empty())
        //        return;

        //    // When popups are stacked, clicking on a lower level popups puts focus back to it and close popups above it.
        //    // Don't close our own child popup windows
        //    int n = 0;
        //    if (g.FocusedWindow != null)
        //    {
        //        for (n = 0; n < g.OpenedPopupStack.Size; n++)
        //        {
        //            ImGuiPopupRef popup = g.OpenedPopupStack[n];
        //            if (popup.Window == null)
        //                continue;
        //            System.Diagnostics.Debug.Assert((popup.Window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0);
        //            if ((popup.Window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0)
        //                continue;

        //            bool has_focus = false;
        //            for (int m = n; m < g.OpenedPopupStack.Size && !has_focus; m++)
        //                has_focus = (g.OpenedPopupStack[m].Window != null && g.OpenedPopupStack[m].Window.RootWindow == g.FocusedWindow.RootWindow);
        //            if (!has_focus)
        //                break;
        //        }
        //    }
        //    if (n < g.OpenedPopupStack.Size)   // This test is not required but it allows to set a useful breakpoint on the line below
        //        g.OpenedPopupStack.resize(n);
        //}

        //ImGuiWindow GetFrontMostModalRootWindow()
        //{
        //    ImGuiState g = State;
        //    if (!g.OpenedPopupStack.empty())
        //    {
        //        ImGuiWindow front_most_popup = g.OpenedPopupStack.back().Window;
        //        if (front_most_popup != null)
        //            if ((front_most_popup.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Modal) != 0)
        //                return front_most_popup;
        //    }
        //    return null;
        //}

        void ClosePopupToLevel(int remaining)
        {
            ImGuiState g = State;
            if (remaining > 0)
                FocusWindow(g.OpenedPopupStack[remaining - 1].Window);
            else
                FocusWindow(g.OpenedPopupStack[0].ParentWindow);
            g.OpenedPopupStack.resize(remaining);
        }

        void ClosePopup(uint id)
        {
            if (!IsPopupOpen(id))
                return;
            ImGuiState g = State;
            ClosePopupToLevel(g.OpenedPopupStack.Size - 1);
        }

        // Close the popup we have begin-ed into.
        void CloseCurrentPopup()
        {
            ImGuiState g = State;
            int popup_idx = g.CurrentPopupStack.Size - 1;
            if (popup_idx < 0 || popup_idx > g.OpenedPopupStack.Size || g.CurrentPopupStack[popup_idx].PopupID != g.OpenedPopupStack[popup_idx].PopupID)
                return;
            while (popup_idx > 0 && g.OpenedPopupStack[popup_idx].Window != null && (g.OpenedPopupStack[popup_idx].Window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu) != 0)
                popup_idx--;
            ClosePopupToLevel(popup_idx);
        }

        void ClearSetNextWindowData()
        {
            ImGuiState g = State;
            g.SetNextWindowPosCond = g.SetNextWindowSizeCond = g.SetNextWindowContentSizeCond = g.SetNextWindowCollapsedCond = 0;
            g.SetNextWindowFocus = false;
        }

        bool BeginPopupEx(string str_id, ImGuiWindowFlags extra_flags)
        {
            ImGuiState g = State;
            ImGuiWindow window = g.CurrentWindow;
            uint id = window.GetID(str_id);
            if (!IsPopupOpen(id))
            {
                ClearSetNextWindowData(); // We behave like Begin() and need to consume those values
                return false;
            }

            PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_WindowRounding, 0.0f);
            ImGuiWindowFlags flags = extra_flags | ImGuiWindowFlags.ImGuiWindowFlags_Popup | ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags.ImGuiWindowFlags_NoMove | ImGuiWindowFlags.ImGuiWindowFlags_NoResize | ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings | ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize;

            string name;
            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu) != 0)
                //ImFormatString(name, 20, "##menu_%d", g.CurrentPopupStack.Size);    // Recycle windows based on depth
                name = string.Format("##menu_{0}", g.CurrentPopupStack.Size);
            else
                //ImFormatString(name, 20, "##popup_%08x", id); // Not recycling, so we can close/open during the same frame
                name = string.Format("##popup_{0:X8}", id);
            float alpha = 1.0f;

            //TODO: not sure if this is right, was passing null in here
            bool throwAway = true;
            bool opened = Begin(name, ref throwAway, new ImVec2(0.0f, 0.0f), alpha, flags);
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) == 0)
                g.CurrentWindow.Flags &= ~ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders;
            if (!opened) // opened can be 'false' when the popup is completely clipped (e.g. zero size display)
                EndPopup();

            return opened;
        }

        bool BeginPopup(string str_id)
        {
            if (State.OpenedPopupStack.Size <= State.CurrentPopupStack.Size)    // Early out for performance
            {
                ClearSetNextWindowData(); // We behave like Begin() and need to consume those values
                return false;
            }
            return BeginPopupEx(str_id, ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders);
        }

        bool BeginPopupModal(string name, ref bool p_opened, ImGuiWindowFlags extra_flags)
        {
            ImGuiState g = State;
            ImGuiWindow window = g.CurrentWindow;
            uint id = window.GetID(name);
            if (!IsPopupOpen(id))
            {
                ClearSetNextWindowData(); // We behave like Begin() and need to consume those values
                return false;
            }

            ImGuiWindowFlags flags = extra_flags | ImGuiWindowFlags.ImGuiWindowFlags_Popup | ImGuiWindowFlags.ImGuiWindowFlags_Modal | ImGuiWindowFlags.ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings;
            bool opened = Begin(name, ref p_opened, new ImVec2(0.0f, 0.0f), -1.0f, flags);
            //TODO: if (!opened || (p_opened.HasValue && !p_opened.Value)) // Opened can be 'false' when the popup is completely clipped (e.g. zero size display)
            if (!opened || !p_opened) // Opened can be 'false' when the popup is completely clipped (e.g. zero size display)
            {
                EndPopup();
                if (opened)
                    ClosePopup(id);
                return false;
            }

            return opened;
        }

        void EndPopup()
        {
            ImGuiWindow window = GetCurrentWindow();
            System.Diagnostics.Debug.Assert((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0);  // Mismatched BeginPopup()/EndPopup() calls
            System.Diagnostics.Debug.Assert(State.CurrentPopupStack.Size > 0);
            End();
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Modal) == 0)
                PopStyleVar();
        }

        // This is a helper to handle the most simple case of associating one named popup to one given widget.
        // 1. If you have many possible popups (for different "instances" of a same widget, or for wholly different widgets), you may be better off handling 
        //    this yourself so you can store data relative to the widget that opened the popup instead of choosing different popup identifiers.
        // 2. If you want right-clicking on the same item to reopen the popup at new location, use the same code replacing IsItemHovered() with IsItemHoveredRect()
        //    and passing true to the OpenPopupEx().
        //    Because: hovering an item in a window below the popup won't normally trigger is hovering behavior/coloring. The pattern of ignoring the fact that 
        //    the item isn't interactable (because it is blocked by the active popup) may useful in some situation when e.g. large canvas as one item, content of menu
        //    driven by click position.
        bool BeginPopupContextItem(string str_id, int mouse_button)
        {
            if (IsItemHovered() && IsMouseClicked(mouse_button))
                OpenPopupEx(str_id, false);
            return BeginPopup(str_id);
        }

        bool BeginPopupContextWindow(bool also_over_items, string str_id, int mouse_button)
        {
            if (str_id == null) str_id = "window_context_menu";
            if (IsMouseHoveringWindow() && IsMouseClicked(mouse_button))
                if (also_over_items || !IsAnyItemHovered())
                    OpenPopupEx(str_id, true);
            return BeginPopup(str_id);
        }

        bool BeginPopupContextVoid(string str_id, int mouse_button)
        {
            if (str_id == null) str_id = "void_context_menu";
            if (!IsMouseHoveringAnyWindow() && IsMouseClicked(mouse_button))
                OpenPopupEx(str_id, true);
            return BeginPopup(str_id);
        }

        // Helper to display logging buttons
        void LogButtons()
        {
            ImGuiState g = State;

            PushID("LogButtons");
            bool log_to_tty = Button("Log To TTY");
            SameLine();
            bool log_to_file = Button("Log To File");
            SameLine();
            bool log_to_clipboard = Button("Log To Clipboard");
            SameLine();

            PushItemWidth(80.0f);
            PushAllowKeyboardFocus(false);
            SliderInt("Depth", ref g.LogAutoExpandMaxDepth, 0, 9, null);
            PopAllowKeyboardFocus();
            PopItemWidth();
            PopID();

            // Start logging at the end of the function so that the buttons don't appear in the log
            if (log_to_tty)
                LogToTTY(g.LogAutoExpandMaxDepth);
            if (log_to_file)
                LogToFile(g.LogAutoExpandMaxDepth, g.IO.LogFilename);
            if (log_to_clipboard)
                LogToClipboard(g.LogAutoExpandMaxDepth);
        }

        // Start logging ImGui output to TTY
        void LogToTTY(int max_depth = -1)
        {
            ImGuiState g = State;
            if (g.LogEnabled)
                return;
            ImGuiWindow window = GetCurrentWindowRead();

            g.LogEnabled = true;
            g.LogStartDepth = window.DC.TreeDepth;

            g.LogFile = new StreamWriter(Console.OpenStandardOutput());
            g.LogFile.AutoFlush = true;
            Console.SetOut(g.LogFile);

            if (max_depth >= 0)
                g.LogAutoExpandMaxDepth = max_depth;

            LogText("Logging to TTY");
        }


        // Start logging ImGui output to given file
        void LogToFile(int max_depth, string filename)
        {
            ImGuiState g = State;
            if (g.LogEnabled)
                return;
            ImGuiWindow window = GetCurrentWindowRead();

            if (string.IsNullOrEmpty(filename))
            {
                filename = g.IO.LogFilename;
                if (string.IsNullOrEmpty(filename))
                    return;
            }

            g.LogFile = System.IO.File.CreateText(filename);
            if (g.LogFile != null)
            {
                System.Diagnostics.Debug.Assert(g.LogFile != null); // Consider this an error
                return;
            }
            g.LogEnabled = true;
            g.LogStartDepth = window.DC.TreeDepth;
            if (max_depth >= 0)
                g.LogAutoExpandMaxDepth = max_depth;

            LogText("Logging to file");
        }

        // Start logging ImGui output to clipboard
        void LogToClipboard(int max_depth = -1)
        {
            ImGuiState g = State;
            if (g.LogEnabled)
                return;
            ImGuiWindow window = GetCurrentWindowRead();

            g.LogEnabled = true;
            g.LogFile = null;
            g.LogStartDepth = window.DC.TreeDepth;
            if (max_depth >= 0)
                g.LogAutoExpandMaxDepth = max_depth;

            LogText("Logging to clipboard");
        }

        void LogFinish()
        {
            ImGuiState g = State;
            if (!g.LogEnabled)
                return;

            LogText(System.Environment.NewLine);
            g.LogEnabled = false;
            if (g.LogFile != null)
            {
                g.LogFile.Flush();
                g.LogFile.Close();
                g.LogFile = null;
            }
            if (g.LogClipboard.Length > 1)
            {
                if (g.IO.SetClipboardTextFn != null)
                    g.IO.SetClipboardTextFn(g.LogClipboard.ToString());
                g.LogClipboard.Clear();
            }
        }

        void LogText(string text)
        {
            ImGuiState g = State;
            if (!g.LogEnabled)
                return;

            if (g.LogFile != null)
            {
                g.LogFile.Write(text);
            }
            else
            {
                g.LogClipboard.Append(text);
            }
        }

        // Pass text data straight to log (without being displayed)
        void LogText(string fmt, params object[] args)
        {
            if (args != null && args.Length > 0)
                fmt = string.Format(fmt, args);

            LogText(fmt);
        }

        // Internal version that takes a position to decide on newline placement and pad items according to their depth.
        // We split text into individual lines to add current tree level padding
        void LogRenderedText(ImVec2 ref_pos, string data, int text = 0, int text_end = -1)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindowRead();

            if (text_end == -1)
                text_end = FindRenderedTextEnd(data, text, text_end);

            bool log_new_line = ref_pos.y > window.DC.LogLinePosY + 1;
            window.DC.LogLinePosY = ref_pos.y;

            int text_remaining = text;
            if (g.LogStartDepth > window.DC.TreeDepth)  // Re-adjust padding if we have popped out of our starting depth
                g.LogStartDepth = window.DC.TreeDepth;
            int tree_depth = (window.DC.TreeDepth - g.LogStartDepth);
            for (;;)
            {
                // Split the string. Each new line (after a '\n') is followed by spacing corresponding to the current depth of our log entry.
                int line_end = text_remaining;
                while (line_end < text_end)
                    if (data[line_end] == '\n')
                        break;
                    else
                        line_end++;
                if (line_end >= text_end)
                    line_end = -1;

                bool is_first_line = (text == text_remaining);
                bool is_last_line = false;
                if (line_end == -1)
                {
                    is_last_line = true;
                    line_end = text_end;
                }
                if (line_end != -1 && !(is_last_line && (line_end - text_remaining) == 0))
                {
                    int char_count = (int)(line_end - text_remaining);
                    if (log_new_line || !is_first_line)
                    {
                        LogText(new string(' ', tree_depth * 4));
                        //really shouldn't be doing substring here but this is logging, who cares
                        LogText(data.Substring(text_remaining, char_count));
                        //LogText("%*s%.*s", new string(' ', tree_depth * 4), "", char_count, text_remaining);
                    }
                    else
                        //   LogText(" %.*s", char_count, text_remaining);
                        LogText(data.Substring(text_remaining, char_count));
                }

                if (is_last_line)
                    break;
                text_remaining = line_end + 1;
            }
        }
    }
}
