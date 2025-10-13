import socket

# Параметры сервера
server_ip = '0.0.0.0'  # Привязываемся ко всем доступным интерфейсам
server_port = 17171    # Порт для прослушивания
buffer_size = 1024     # Размер буфера для приема данных

try:
    # Создаем UDP-сокет
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    # Привязываем сокет к указанному IP и порту
    server_socket.bind((server_ip, server_port))
    
    print(f"UDP сервер запущен и слушает порт {server_port}")
    
    while True:
        # Получаем данные от клиента
        data, client_address = server_socket.recvfrom(buffer_size)
        
        # Декодируем полученные данные
        message = data.decode('utf-8')
        
        # Выводим информацию о полученном сообщении
        print(f"Получено сообщение от {client_address}: {message}")

except KeyboardInterrupt:
    print("\nСервер остановлен пользователем")
except Exception as e:
    print(f"Произошла ошибка: {e}")
finally:
    # Закрываем сокет при завершении работы
    server_socket.close()
    print("Сокет закрыт")
