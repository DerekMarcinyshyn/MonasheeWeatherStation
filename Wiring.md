# Wiring Notes

DHT22 Sensor
------------

http://www.seeedstudio.com/depot/grove-temperaturehumidity-sensor-pro-p-838.html

* Black = GND
* Red = 5v
* White = not used
* Yellow = Digital 0 + Digital 1 

BMP085 Sensor
-------------

http://www.seeedstudio.com/depot/grove-barometer-sensor-p-1199.html

* Black = GND
* Red = 5v
* White = SD
* Yellow = SC

Sparkfun Weather Meters
-----------------------

https://www.sparkfun.com/products/8942

Anemometer
----------

Middle two wires coming from Anemometer and Wind Vane

* Blue = GND Bus -> Bus
* Purple = Green -> Digital 12

Wind Vane
---------

Outer two wires coming from Anemometer and Wind Vane

* Grey = GND Bus -> GND
* Green ---- 10k resistor ---- 3.3v
*      +
*	   +---- Yellow to Analog 2

Rain Gauge
----------

* Black = GND Bus -> GND
* White = Blue -> Digital 10