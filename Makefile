CC=clang-cl
DIR=${CURDIR}

program:
	$(CC) -Zi $(DIR)/main.cpp -o test.exe

.PHONY: program

clean:
	rm $(DIR)/program