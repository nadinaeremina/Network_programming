#include <iostream>
#include <string>
#include <WinSock2.h>
#include <WS2tcpip.h>
#pragma comment(lib, "Ws2_32.lib")

using namespace std;

void main()
{
	// максим возможная длина строки 
	const int MAXSTRLEN = 256;

	// инф-цию об инициализации сокетов нужно где-то хранить - поэтому создаем стр-ру
	WSADATA wsaData;

	// здесь будет храниться дискриптор сокета клиента
	SOCKET dsock;

	// для хранения локального адреса и порта инициализации сокета
	sockaddr_in addr;


	// сообщаем инф-цию о windows-socket, который мы исп-ем // инициализируем win-socket
	WSAStartup(MAKEWORD(2, 2), &wsaData);
	// если мы передаем указатель - скорре всего мы не передаем в ф-цию данные, которые нужны будут
	// а мы передаем в нее стр-ру, в которую у нас ф-ция положит эти данные

	// получаем дискриптор сокета клиента - создаем сокет
	dsock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	// тип создаваемого сокета 'Sream' - TCP

	// настраиваем адрес
	addr.sin_family = AF_INET;
	inet_pton(AF_INET, "0.0.0.0", &addr.sin_addr); // из адреса достаем его sin_addr
	// в sin_addr  будет помещен некий ip-адрес 
	// sin_addr относится к семейству in_addr - соот-но туда будет помещен именно этот тип данных - in_addr
	// ip хранится не ввиде строки - а в опред типе данных, который будет туда помещен

	// адресу назначаем sin_port
	addr.sin_port = htons(20000);

	// вместо 'accept' здесь будет 'connect'
	connect(dsock, (SOCKADDR*)&addr, sizeof(addr));

	// нужно передать сообщение серверу - создаем буфер
	char buf[MAXSTRLEN];
	const char* text = "Hello, server!";
	cout << "\nPress Enter to send hello to the server\n";

	// дожидаемся нажатия ENTER
	cin.get(); 

	// на подключенный сокет отправляем сообщение
	send(dsock, text, strlen(text), 0);

	// теперь сервер должен прислать сообщение // запоминаем размер полученного сообщения
	int i = recv(dsock, buf, MAXSTRLEN, 0);

	// записываем символ окончания строки
	buf[i] = '\0';

	cout << buf << endl;

	// закрываем сокет
	closesocket(dsock);

	WSACleanup();
	system("pause");
}
