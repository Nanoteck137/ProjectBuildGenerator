#include "test.h"

#include <stdio.h>

int Test2(const char *str, int a)
{
    printf("String: %s\nA:%d\n", str, a);
    return a;
}