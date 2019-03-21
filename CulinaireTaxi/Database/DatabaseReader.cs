﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CulinaireTaxi.Database
{

    public class DatabaseReader {

	private MySqlConnection _mysqlConnection;
	public bool _mysqlConnectedState;
    
	public DatabaseReader(string _ipAdress, string _port, string _databaseName, string _username, string _password) {

	    // create connection
	    _mysqlConnection = new MySqlConnection(
		"user id=" + _username + ";" +
		"port=" + _port + ";" +
		"password=" + _password + ";" +
		"server=" + _ipAdress + ";" +
		"DATABASE=" + _databaseName + ";" +
		"connection timeout=5"
	    );

	    // open connection
	    try {
		_mysqlConnection.Open();
		_mysqlConnectedState = true;
	    }
	    catch (Exception) {
		_mysqlConnectedState = false;
		throw;
	    }
	}

	public bool hasTable(string _tableName) {
	    if (!_mysqlConnectedState)
		return false;

	    MySqlCommand command = new MySqlCommand("SHOW TABLES LIKE '" + _tableName + "';", _mysqlConnection);
	    try {
		using (MySqlDataReader dataReader = command.ExecuteReader()) {
		    return dataReader.HasRows;
		}
	    }
	    catch (System.Exception e) {
		Console.WriteLine(e.ToString());
		return false;
	    }
	}

	// create new table in database
	public void createTable(string _tableName, SortedList<string, MySqlDataType> _namesAndTypes) {
	    if (!_mysqlConnectedState)
		return;

	    if (hasTable(_tableName))
		return;

	    string command = "CREATE TABLE `" + _tableName + "` (";
	    for (int i = 0; i < _namesAndTypes.Count; i++) {
		command += '`' + _namesAndTypes.Keys[i] + "` ";

		switch (_namesAndTypes.Values[i]) {
		    case MySqlDataType.AutoIncrementId:
			command += "INT NOT NULL AUTO_INCREMENT PRIMARY KEY";
			break;
		    case MySqlDataType.Boolean:
			command += "BOOLEAN NOT NULL";
			break;
		    case MySqlDataType.Float:
			command += "FLOAT NOT NULL";
			break;
		    case MySqlDataType.Integer:
			command += "INT NOT NULL";
			break;
		    case MySqlDataType.Text:
			command += "TEXT NOT NULL";
			break;
		}

		if (i < _namesAndTypes.Count - 1)
		    command += ", ";
	    }
	    command += ") ENGINE = InnoDB";
	    MySqlCommand sqlCommand = new MySqlCommand(command, _mysqlConnection);
	    sqlCommand.ExecuteNonQuery();
	}

	public bool containsData(string _tableName, string _where) {
	    if (!_mysqlConnectedState)
		return false;

	    if (!hasTable(_tableName))
		return false;

	    string command = "SELECT * FROM " + _tableName + " Where " + _where;
	    MySqlCommand sqlCommand = new MySqlCommand(command, _mysqlConnection);

	    using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
	    {
		return dataReader.HasRows;
	    }
	}

	public void addData(string _tableName, SortedList<string, string> _values) {
	    if (!_mysqlConnectedState)
		return;

	    if (!hasTable(_tableName))
		return;

	    string command = "INSERT INTO `" + _tableName + "` (";

	    for (int i = 0; i < _values.Count; i++)
	    {
		command += "`" + _values.Keys[i] + "`";
		if (i < _values.Count - 1)
		    command += ',';
	    }
	    command += ") VALUES (";

	    for (int i = 0; i < _values.Count; i++)
	    {
		command += "'" + _values.Values[i] + "'";
		if (i < _values.Count - 1)
		    command += ',';
	    }
	    command += ");";

	    MySqlCommand sqlCommand = new MySqlCommand(command, _mysqlConnection);
	    sqlCommand.ExecuteNonQuery();
	}
    
	public string getData(string _tableName, string _column, string _where) {
	    if (!_mysqlConnectedState)
		return "";

	    if (!hasTable(_tableName))
		return "";

	    string command = "SELECT * FROM " + _tableName + " Where " + _where;
	    MySqlCommand sqlCommand = new MySqlCommand(command, _mysqlConnection);

	    string returnvalue = "";

	    using (MySqlDataReader dataReader = sqlCommand.ExecuteReader()) {
		bool addCommas = false;
		while (dataReader.Read()){
		    if (addCommas)
			returnvalue += ",";
		    returnvalue += dataReader[_column];
		    addCommas = true;
		}
	    }
	    return returnvalue;
	}

	public bool isConnected() {
	    return _mysqlConnectedState;
	}
    
	public enum MySqlDataType {
	    AutoIncrementId,
	    Text,
	    Integer,
	    Float,
	    Boolean
	}
    }

}