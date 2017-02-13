//-----------------------------------------------------------------------------
// ImGuiStorage
//-----------------------------------------------------------------------------

// Helper: Key->value storage
void ImGuiStorage::Clear()
{
	Data.clear();
}

// std::lower_bound but without the bullshit
static ImVector<ImGuiStorage::Pair>::iterator LowerBound(ImVector<ImGuiStorage::Pair>& data, uint key)
{
	ImVector<ImGuiStorage::Pair>::iterator first = data.begin();
	ImVector<ImGuiStorage::Pair>::iterator last = data.end();
	int count = (int)(last - first);
	while (count > 0)
	{
		int count2 = count / 2;
		ImVector<ImGuiStorage::Pair>::iterator mid = first + count2;
		if (mid->key < key)
		{
			first = ++mid;
			count -= count2 + 1;
		}
		else
		{
			count = count2;
		}
	}
	return first;
}

int ImGuiStorage::GetInt(uint key, int default_val) const
{
	ImVector<Pair>::iterator it = LowerBound(const_cast<ImVector<ImGuiStorage::Pair>&>(Data), key);
	if (it == Data.end() || it->key != key)
		return default_val;
	return it->val_i;
}

float ImGuiStorage::GetFloat(uint key, float default_val) const
{
	ImVector<Pair>::iterator it = LowerBound(const_cast<ImVector<ImGuiStorage::Pair>&>(Data), key);
	if (it == Data.end() || it->key != key)
		return default_val;
	return it->val_f;
}

void* ImGuiStorage::GetVoidPtr(uint key) const
{
	ImVector<Pair>::iterator it = LowerBound(const_cast<ImVector<ImGuiStorage::Pair>&>(Data), key);
	if (it == Data.end() || it->key != key)
		return NULL;
	return it->val_p;
}

// References are only valid until a new value is added to the storage. Calling a Set***() function or a Get***Ref() function invalidates the pointer.
int* ImGuiStorage::GetIntRef(uint key, int default_val)
{
	ImVector<Pair>::iterator it = LowerBound(Data, key);
	if (it == Data.end() || it->key != key)
		it = Data.insert(it, Pair(key, default_val));
	return &it->val_i;
}

float* ImGuiStorage::GetFloatRef(uint key, float default_val)
{
	ImVector<Pair>::iterator it = LowerBound(Data, key);
	if (it == Data.end() || it->key != key)
		it = Data.insert(it, Pair(key, default_val));
	return &it->val_f;
}

void** ImGuiStorage::GetVoidPtrRef(uint key, void* default_val)
{
	ImVector<Pair>::iterator it = LowerBound(Data, key);
	if (it == Data.end() || it->key != key)
		it = Data.insert(it, Pair(key, default_val));
	return &it->val_p;
}

// FIXME-OPT: Need a way to reuse the result of lower_bound when doing GetInt()/SetInt() - not too bad because it only happens on explicit interaction (maximum one a frame)
void ImGuiStorage::SetInt(uint key, int val)
{
	ImVector<Pair>::iterator it = LowerBound(Data, key);
	if (it == Data.end() || it->key != key)
	{
		Data.insert(it, Pair(key, val));
		return;
	}
	it->val_i = val;
}

void ImGuiStorage::SetFloat(uint key, float val)
{
	ImVector<Pair>::iterator it = LowerBound(Data, key);
	if (it == Data.end() || it->key != key)
	{
		Data.insert(it, Pair(key, val));
		return;
	}
	it->val_f = val;
}

void ImGuiStorage::SetVoidPtr(uint key, void* val)
{
	ImVector<Pair>::iterator it = LowerBound(Data, key);
	if (it == Data.end() || it->key != key)
	{
		Data.insert(it, Pair(key, val));
		return;
	}
	it->val_p = val;
}

void ImGuiStorage::SetAllInt(int v)
{
	for (int i = 0; i < Data.Size; i++)
		Data[i].val_i = v;
}