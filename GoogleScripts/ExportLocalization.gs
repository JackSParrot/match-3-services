function exportLocalization() 
{
  let jsonResult = {};
  let spreadsheet = SpreadsheetApp.openById("1lTFrx4GFYRE704ZNieFR-7YyMI3fPI2bRU4yCZwNkD8").getSheets()[0];
  const data = spreadsheet.getDataRange().getValues();

  for(let i = 1; i < data.length;++i)
  {
    for(let j = 1; j < data[0].length;++j)
    {
      if(jsonResult[data[0][j]] == undefined)
      {
        jsonResult[data[0][j]] = {data:[]};
      }
      jsonResult[data[0][j]]["data"].push({ key: data[i][0], value: data[i][j]});
    }
  }
  Logger.log(JSON.stringify(jsonResult));
  return JSON.stringify(jsonResult);
}

function doGet(r)
{
  return ContentService.createTextOutput(exportLocalization());
}