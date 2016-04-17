# KWIC for CSharp

## Sample Code
```CS
string text = File.ReadAllText(@"TheLastQuestion.txt");

var kwic = new KWIC(text)
{
    ForwardMargin = 30,
    BackwardMargin = 30
};
var results = kwic.Concordance("question");

foreach (var result in results)
{
    Console.WriteLine(result);
}

```

## Result
```
                         last question was asked for the first time, 
  stepped into the light. The question came about as a result of a  
symbols and operations into a question which, in words, might have  
men thereupon returned to the question of the report they were to   
       matter nor energy. The question of its size and Nature no    
 for the sake of the one last question that it had never answered   
   years before had asked the question of a computer that was to AC 
answered, and until this last question was answered also, AC might  
```

If you want to excute code, [Run in .NET Fiddler](https://dotnetfiddle.net/JB8QNo)

Author : [Junho Park](mailto:huinalam@gmail.com)
Yeungnam University, Computer Engineering