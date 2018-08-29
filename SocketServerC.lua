-- load namespace
local socket = require("socket")
-- create a TCP socket and bind it to the local host, at any port
local server = assert(socket.bind("*", 20000))
-- find out which port the OS chose for us
local ip, port = server:getsockname()
-- print a message informing what's up
print("Please telnet to localhost on port " .. port)
print("After connecting, you have 10s to enter a line to be echoed")

print("waiting for the client to connect")
client, errr = server:accept() -- Waits for a remote connection on the server object and returns a 
  -- client object representing that connection return a client object
  -- client => is an object if the connection is successful if not we get nil (in case of error)
  -- err => the error message or nil if there is no error
 
 str = ""
  -- make sure we don't block waiting for this client's line
   client:settimeout(50)

   -- my strategy is that the first message received in theserver will be the number of bytes
   
if client ~= nil then 
	print('The TCP connection is successful') 
	
			--str, data_error = client:receive() 
	 
		while 1 do
			data, data_error = client:receive(num) 
			-- if data is a number then the output of the "tonumber()" will be a number otherwise we receive "nil"
			num = tonumber(data) 
			
			if num~= nil then
				numBytes = num
				print('---------------------- The number of Bytes is:' , numBytes)
			elseif num == nil then
				str = data
					print('---------------------- The message is:')
					print(str)
				break
			end
		end	
end
		
-- the strategy here is at first I delete the output file of ArtTreeKS (out_result.lua) and then run the ArtTreeKS and wait till the output file is created.
-- then send the message to Unity

-- remove ArtTreeKS output
os.remove('out_result.lua')
print('------------- > out_result is removed')
-- run ArtTreeKS
f = loadstring(str);
f()
print('------------- > ArtTreeKS ran')

while io.open('out_result.lua', "r") do
	 -- At this point ArtTreeKS has created the output file but just to make sure everything is OK, I wait 3 more seconds
	 print('------------- > out_result is created')
	socket.sleep(3)
	
	-- extract the joint values and axes from out_result.lua and send them to Unity
	outResult = io.open('out_result.lua', "r")
	outStr = outResult:read("*all")
	outResult:close()
	print(outStr)
	
	-- send msg to Unity
	print('------------- > Lua is sending the msg')
	--client:send("helloooooo from Luaaaaaaa")
	client:send(outStr)
	client:close()
	
	break
end

 --local numBytes, numBytes_error = client:receive() -- "*a" this option waits for the socket to close which is not good for my purpose as I want to send some messges back
 -- data is going to be string, in case of error data is going to be nil
 -- err => the error message, or nil if no error occurred

-- client:close()






