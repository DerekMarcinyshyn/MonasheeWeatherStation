# Wiring Notes

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

Humidity
--------

https://www.sparkfun.com/products/9890

* Black = GND
* Red = 5v
* Orange = Analog 0

DS18B20
-------

https://solarbotics.com/product/52210/

* Red = 5v
* White = 5v -> 4.7k resistor -> Digital 4
* Silver = GND