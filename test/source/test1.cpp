#include "test.h"
#include <stdio.h>
int Test1(int a, int b)
{
    return a * b + 1;
}

int Test2(const char *str, int a) 
{
    printf("String: %s\nA:%d\n", str, a);
    return a;
}