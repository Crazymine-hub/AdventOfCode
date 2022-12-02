## Day 8 Notes
## Mapping
_ by example of `acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf`_

## Group Usage
```
            +-=# d
             ______ 
            |  1   |
       *-= e|2    4|a &#*=
            |  8   |
       *+= f ------
            |      |
        =  g|16  64|b &#*-=
            |__32__|
            +-=  c
mask: abcdefg
```
## Groups:
|char|nr |wires  |
|----|---|-------|
|&   |2  |ab~~cdefg~~|
|#   |3  |ab~~c~~d~~efg~~|
|\*  |4  |ab~~cd~~ef~~g~~|
|+   |5  |~~ab~~cd~~e~~f~~g~~|
|-   |6  |~~a~~bcde~~fg~~|
|=   |7  |abcdefg|

## Segment usage
### by segment
|Segment|Numbers Used  |Numbers not used|
|-------|--------------|----------------|
|1      |`0-23-56789`  |`-1--4-----`    |
|2      |`0---456-89`  |`-123---7--`    |
|4      |`--23456-89`  |`01-----7--`    |
|8      |`01234--789`  |`-----56---`    |
|16     |`0-2---6-8-`  |`-1-345-7-9`    |
|32     |`0-23-56-89`  |`-1--4--7--`    |
|64     |`01-3456789`  |`--2-------`    |

### by digit
Wich segments are on (`1`) for each digit?
```
0=1111011
1=1001000
2=0111101
3=1101101
4=1001110
5=1100111
6=1110111
7=1001001
8=1111111
9=1101111
```

## Segment dependency
Enable the segment in the row. AND All numbers from above together that need this segment.
Wich segments are on?
```1  0000001
2  1000010
4  0000100
8  0001000
16 0110001
32 0100001
64 1000000
```
Pretty pointless, right?


## Example Number mapping
wich number could the string resolve to?

|String|Numbers|
|------:|-------|
|acedgfb|8      |
|cdfbe |235    |
|gcdfa |235    |
|fbcad |235    |
|dab   |7      |
|cefabd|069    |
|cdfgeb|069    |
|cagedb|069    |
|eafb  |4      |
|ab    |1      |