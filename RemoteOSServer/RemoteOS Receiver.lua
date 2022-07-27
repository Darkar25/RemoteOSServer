local host = "localhost"
local port = 4466

-- Modified JSON encoding library, https://github.com/rxi/json.lua/blob/master/json.lua

json = {}
local encode
local function encode_nil(val)
  return "null"
end
local function encode_table(val, stack)
	local res = {}
	local array = true
	local length = 0
	local nLen = 0
	for k,v in pairs(val) do
		if (type(k) ~= "number" or k<=0) and not (k == "n" and type(v) == "number") then
			array = nil
			break
		end
		if k > length then 
			length = k
		end
		if k == "n" and type(v) == "number" then
			nLen = v
		end
	end
	if array then
		if nLen > length then
			length = nLen
		end
		for i=1,length do
			table.insert(res, encode(val[i], stack))
		end
		return table.concat({"[", table.concat(res, ","), "]"})
	end
    for k, v in pairs(val) do
		table.insert(res, table.concal({encode(k, stack), ":", encode(v, stack)}))
    end
    return table.concat({"{", table.concat(res, ","), "}"})
end
local function encode_number(val)
	return (val ~= val and "NaN") or (val <= -math.huge and "-Infinity") or (val >= math.huge and "Infinity") or tostring(val)
end
local function encode_string(val)
  return string.format("%q", val)
end
local type_func_map = {
  [ "nil"     ] = encode_nil,
  [ "table"   ] = encode_table,
  [ "string"  ] = encode_string,
  [ "number"  ] = encode_number,
  [ "boolean" ] = tostring,
}
encode = function(val)
  return type_func_map[type(val)](val)
end
function json.encode(val)
  return (encode(val))
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
							if not s then break end
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
								if not sock.write(table.concat({"r\0",execid,"\0",e,"\0",result,"\r\n"})) then break end
							end
						end
					elseif not sock.write(table.concat({"e\0",json.encode({table.unpack(event)}),"\r\n"})) then break end
				end
			end
			sock.close()
		end
	end
end