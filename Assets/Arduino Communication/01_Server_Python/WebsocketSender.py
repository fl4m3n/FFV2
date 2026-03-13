import asyncio
import websockets
import time

async def send():
    uri = "ws://localhost:8081"
    async with websockets.connect(uri) as ws:
        while True:
            msg = await asyncio.to_thread(input, "Input message (leave empty for parachute button): ")
            if (not msg):
                await ws.send("buttonP:1")
                time.sleep(0.2)
                await ws.send("buttonP:0")
            else:
                await ws.send(msg)

asyncio.run(send())