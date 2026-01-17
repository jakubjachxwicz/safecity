#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Skrypt inicjalizacji bazy danych
- Tworzy tabele
- Rejestruje admina przez API (jeśli API działa)
"""

import psycopg2
import sys
import requests
import time

DB_CONFIG = {
    'dbname': 'safecitydb',
    'user': 'postgres',
    'password': 'drone123',
    'host': 'localhost',
    'port': '5432'
}

API_URL = 'http://localhost:5102'

ADMIN_DATA = {
    'username': 'admin',
    'email': 'admin@safecity.local',
    'password': 'Retsuzik123!'
}

def create_connection():
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        return conn
    except psycopg2.Error as e:
        print(f"❌ Błąd połączenia: {e}")
        sys.exit(1)

def create_tables(cursor):
    print("Tworzenie tabel...\n")
    
    cursor.execute("""
        CREATE TABLE IF NOT EXISTS users (
            id UUID PRIMARY KEY,
            username VARCHAR(50) NOT NULL UNIQUE,
            email VARCHAR(100) NOT NULL UNIQUE,
            password_hash TEXT NOT NULL,
            role VARCHAR(20) NOT NULL DEFAULT 'user',
            is_banned BOOLEAN NOT NULL DEFAULT false,
            created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
    """)
    print("Tabela 'users'")
    
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);")
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);")
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_users_created_at ON users(created_at);")
    
    cursor.execute("""
        CREATE TABLE IF NOT EXISTS reports (
            id UUID PRIMARY KEY,
            reported_at TIMESTAMP NOT NULL,
            latitude DOUBLE PRECISION NOT NULL,
            longitude DOUBLE PRECISION NOT NULL,
            message TEXT,
            user_id UUID,
            ip_address VARCHAR(45) NOT NULL
        );
    """)
    print("Tabela 'reports'")
    
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_reports_reported_at ON reports(reported_at);")
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_reports_user_id ON reports(user_id);")
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_reports_ip_address ON reports(ip_address);")
    cursor.execute("CREATE INDEX IF NOT EXISTS idx_reports_location ON reports(latitude, longitude);")

def add_created_at_if_missing(cursor):
    print("\nAktualizacja...")
    
    cursor.execute("""
        SELECT column_name FROM information_schema.columns 
        WHERE table_name = 'users' AND column_name = 'created_at'
    """)
    
    if cursor.fetchone() is None:
        cursor.execute("ALTER TABLE users ADD COLUMN created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP")
        cursor.execute("CREATE INDEX IF NOT EXISTS idx_users_created_at ON users(created_at)")
        print("Dodano pole created_at")
    else:
        print("Struktura OK")

def check_if_api_running():
    """Sprawdź czy API działa"""
    try:
        response = requests.get(f"{API_URL}/health", timeout=2)
        return response.status_code == 200
    except:
        return False

def register_admin_via_api():
    """Rejestruje admina przez API"""
    try:
        print("\n👤 Rejestrowanie admina przez API...")
        
        response = requests.post(
            f"{API_URL}/api/users/register",
            json=ADMIN_DATA,
            timeout=5
        )
        
        if response.status_code == 201:
            print("Admin zarejestrowany przez API!")
            return True
        elif response.status_code == 400:
            print("  Admin już istnieje")
            return False
        else:
            print(f"Błąd rejestracji: {response.status_code}")
            return False
            
    except requests.exceptions.RequestException as e:
        print(f" Nie można połączyć się z API")
        return False

def promote_to_admin(cursor, username):
    """Nadaj rolę admina"""
    cursor.execute("UPDATE users SET role = 'admin' WHERE username = %s", (username,))
    print(f"Użytkownik '{username}' jest teraz adminem!")

def main():
    print("\n" + "="*70)
    print(" SafeCity - Inicjalizacja bazy danych")
    print("="*70)
    
    conn = create_connection()
    cursor = conn.cursor()
    
    try:
        # Utwórz tabele
        create_tables(cursor)
        add_created_at_if_missing(cursor)
        conn.commit()
        
        print("\n" + "="*70)
        print("Baza danych gotowa!")
        print("="*70)
        
        # Sprawdź czy są użytkownicy
        cursor.execute("SELECT COUNT(*) FROM users WHERE username = %s", (ADMIN_DATA['username'],))
        admin_exists = cursor.fetchone()[0] > 0
        
        if not admin_exists:
            print("\nTworzenie admina...")
            
            # Sprawdź czy API działa
            if check_if_api_running():
                # API działa - zarejestruj przez API
                if register_admin_via_api():
                    time.sleep(1)  # Poczekaj chwilę
                    promote_to_admin(cursor, ADMIN_DATA['username'])
                    conn.commit()
                    
                    print("\n" + "="*70)
                    print("Admin gotowy!")
                    print("="*70)
                    print(f"Username: {ADMIN_DATA['username']}")
                    print(f"Password: {ADMIN_DATA['password']}")
                    print("="*70)
            else:
                # API nie działa - pokaż instrukcje
                print("⚠️  API nie działa. Wykonaj ręcznie:")
                print("")
                print("1. Uruchom API: dotnet run")
                print("")
                print("2. Zarejestruj admina:")
                print("   POST http://localhost:5102/api/users/register")
                print(f"   Body: {ADMIN_DATA}")
                print("")
                print("3. Nadaj rolę admina:")
                print(f"   UPDATE users SET role = 'admin' WHERE username = '{ADMIN_DATA['username']}';")
        else:
            print("\nℹ️  Admin już istnieje")
        
        print("")
        
    except psycopg2.Error as e:
        conn.rollback()
        print(f"\n❌ Błąd: {e}")
        sys.exit(1)
    finally:
        cursor.close()
        conn.close()

if __name__ == "__main__":
    main()