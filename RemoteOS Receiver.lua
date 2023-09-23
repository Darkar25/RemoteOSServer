local host = "localhost"
local port = 4466

-- Modified & Minified JSON encoding library, https://github.com/rxi/json.lua/blob/master/json.lua

local json={}
local e
e=function(v)return({
["nil"]=function()return"null"end,
["table"]=function(v)local r={}if#v>0 and next(v,#v)==nil then for i=1,#v do r[i]=e(v[i])end return"["..table.concat(r,",").."]"end for k,j in pairs(v)do r[#r+1]=e(k)..":"..e(j)end return"{"..table.concat(r,",").."}"end,
["string"]=function(v)return string.format("%q",v)end,
["number"]=function(v)return v~=v and"NaN"or v<=-math.huge and"-Infinity"or v>=math.huge and"Infinity"or tostring(v)end,
["boolean"]=tostring})[type(v)](v)end
function json.encode(v)return e(v)end

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
							if d ~= nil and #d > 0 then
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