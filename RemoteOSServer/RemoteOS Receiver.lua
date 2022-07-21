local host = "localhost"
local port = 4466

-- JSON encoding library, https://github.com/rxi/json.lua/blob/master/json.lua
json = {}
local encode
local escape_char_map = {
  [ "\\" ] = "\\",
  [ "\"" ] = "\"",
  [ "\b" ] = "b",
  [ "\f" ] = "f",
  [ "\n" ] = "n",
  [ "\r" ] = "r",
  [ "\t" ] = "t",
}
local function escape_char(c)
  return "\\" .. (escape_char_map[c] or string.format("u%04x", c:byte()))
end
local function encode_nil(val)
  return "null"
end
local function encode_table(val, stack)
  local res = {}
  stack = stack or {}
  stack[val] = true
  local array = true
  local length = 0
	local nLen = 0
  for k,v in pairs(val) do
		if (type(k) ~= "number" or k<=0) and not (k == "n" and type(v) == "number") then
			array = nil
			break
		else
			if k > length then 
				length = k
			end
			if k == "n" and type(v) == "number" then
				nLen = v
			end
		end
  end
  if array then
		if nLen > length then
			length = nLen
		end
    for i=1,length do
      table.insert(res, encode(val[i], stack))
    end
    stack[val] = nil
    return "[" .. table.concat(res, ",") .. "]"
  else
    for k, v in pairs(val) do
      table.insert(res, encode(k, stack) .. ":" .. encode(v, stack))
    end
    stack[val] = nil
    return "{" .. table.concat(res, ",") .. "}"
  end
end
local function encode_string(val)
  return '"' .. val:gsub('[%z\1-\31\\"]', escape_char) .. '"'
end
local type_func_map = {
  [ "nil"     ] = encode_nil,
  [ "table"   ] = encode_table,
  [ "string"  ] = encode_string,
  [ "number"  ] = tostring,
  [ "boolean" ] = tostring,
}
encode = function(val, stack)
  local t = type(val)
  local f = type_func_map[t]
  if f then
    return f(val, stack)
  end
end
function json.encode(val)
  return ( encode(val) )
end

-- RemoteOS main program

local component = component or require("component")
local computer = computer or require("computer")

local inet = component.proxy(component.list("internet")())
local global = {}
local sockid = ""
while true do
	local sock, reason = inet.connect(host, port)
	if sock then
		local connected = false
		while not connected do
			res, err = sock.finishConnect()
			if err then break end
			connected = res
		end
		if connected then
			sockid = sock.id()
			local exec = ""
			while sock.finishConnect() do
				local event = table.pack(computer.pullSignal(.1))
				if event[1] then
					if event[1] == "internet_ready" and event[3] == sockid then
						while true do
							local d = sock.read()
							if d ~= nim and #d > 0 then
								exec = exec .. d
							else break end
						end
						while true do
							local s, _ = exec:find("\r\n", 1, true)
							if s then
								local tmp = exec:sub(1, s - 1)
								exec = exec:sub(s + 2)
								s, _ = tmp:find("\0", 1, true)
								if s then
									local execid = tmp:sub(1, s - 1)
									local command = tmp:sub(s + 1)
									local res, err = load(command, "=stdin", nil, setmetatable({json=json, global=global, component=component, computer=computer}, {__index=_G}))
									local result = "false"
									local e = ""
									if res then
										local success, res = pcall(res)
										if success then
											result = res or "true"
										else
											e = res or e
										end
									else
										e = err or e
									end
									if not sock.write("r\0"..execid.."\0"..result.."\0"..e.."\r\n") then break end
								end
							else break end
						end
					elseif not sock.write("e\0"..json.encode({table.unpack(event)}).."\r\n") then break end
				end
			end
			sock.close()
		end
	end
end