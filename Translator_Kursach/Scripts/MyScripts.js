$(document).ready(function () {
    var runBtn = document.querySelector('.start-btn');
    var grammar = document.querySelector('#grammarText');
    runBtn.addEventListener('click', function () {
        $.ajax({
            type:'POST',
            url: "../Home/RelationTable/",
            async: false,
            data:{
                grammarText: grammar.value
            },
            //onAjaxExecuteSuccess
            beforeSend: function(){
                var loader = document.querySelector('#loader');
                loader.style.display = 'block';
            },
            success:RelationTableBilder
        });
        alert(1);
        //LexemAnalyzer();
    });
});

function RelationTableBilder(result) {
    var loader = document.querySelector('#loader');
    loader.style.display = 'none';
    var tableBox = document.querySelector('.table-box');
    tableBox.style.display = 'block';
    var h3 = document.querySelector('#relation-capture');
    h3.style.display = 'block';
    var table = document.createElement('table');
    var tbody = document.createElement('tbody');
    table.appendChild(tbody);
    var tr = document.createElement('tr');
    tbody.appendChild(tr);
    var td = document.createElement('td');
    td.style.border = '1px solid black';
    td.innerText = '...';
    tr.appendChild(td);
    console.log(result.matrix);
    for (var i = 0; i < result.labels.length; i++) {
        var td = document.createElement('td');
        td.style.border = '1px solid black';
        td.innerText = result.labels[i];
        tr.appendChild(td);
    }

    for (var i = 0; i < result.labels.length; i++) {
        var tr = document.createElement('tr');
        tbody.appendChild(tr);
        var td = document.createElement('td');
        td.style.border = '1px solid black';
        td.innerText = result.labels[i];
        tr.appendChild(td);
        for (var j = 0; j < result.labels.length; j++) {
            var td = document.createElement('td');
            td.style.border = '1px solid black';
            td.innerText = result.matrix[i*result.labels.length + j];
            tr.appendChild(td);
        }
    }

    tableBox.appendChild(table);
    console.log(table);
    ConflictChecker();
    //alert(1);

}

function ConflictChecker() {
    var cells = document.querySelectorAll('.table-box td');
    console.log(cells);
    var length = Math.sqrt(cells.length);

    for (var i = 1; i < length; i++) {
        for (var j = 1; j < length; j++) {
            var index = length*i+j;
            if (cells[index].innerText != '' && cells[index].innerText != '>' && cells[index].innerText != '<' && cells[index].innerText != '=') {
                cells[index].style.backgroundColor = 'red';
            }
        }
    }

    LexemAnalyzer();
}

function LexemAnalyzer() {
    var sourceText = document.querySelector('#codeText').value;
    $.ajax({
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        url: "../Home/Analyzer/",
        dataType: "json",
        async:false,
        data: "{'sourceText': '" + sourceText + "'}",
        beforeSend:function(){
            var loader = document.querySelector('#loader');
            loader.style.display = 'block';
        },
        success:onAnalyzeSuccess
    });
}

function onAnalyzeSuccess(result) {
    var loader = document.querySelector('#loader');
    loader.style.display = 'none';
    var console = document.querySelector('#console');
    console.value += "Lexical analyzer: " + result.lexemAnalyzeResult + '\n';
    var tableDiv = document.querySelector('.lexem-table-token');
    var table = document.createElement('table');
    var theadLexem = document.createElement('thead');
    var th1Lexem = document.createElement('th');
    th1Lexem.innerText = 'Id';
    var th2Lexem = document.createElement('th');
    th2Lexem.innerText="Code"
    var th3Lexem = document.createElement('th');
    th3Lexem.innerText = "Row";
    var th4Lexem = document.createElement('th');
    th4Lexem.innerText = "Token";
    var th5Lexem = document.createElement('th');
    th5Lexem.innerText = "Code Idn/Const";
    theadLexem.appendChild(th1Lexem);
    theadLexem.appendChild(th2Lexem);
    theadLexem.appendChild(th3Lexem);
    theadLexem.appendChild(th4Lexem);
    theadLexem.appendChild(th5Lexem);
    table.appendChild(theadLexem);
    for (var i = 0; i < result.tables._OutLexem.length; i++) {
        var tr = document.createElement('tr');
        var td1 = document.createElement('td');
        td1.innerText = result.tables._OutLexem[i].Id;
        var td2 = document.createElement('td');
        td2.innerText = result.tables._OutLexem[i].Kod;
        var td3 = document.createElement('td');
        td3.innerText = result.tables._OutLexem[i].Row;
        var td4 = document.createElement('td');
        td4.innerText = result.tables._OutLexem[i].Token;
        var td5 = document.createElement('td');
        td5.innerText = result.tables._OutLexem[i].KodIdn_Konst;
        tr.appendChild(td1);
        tr.appendChild(td2);
        tr.appendChild(td3);
        tr.appendChild(td4);
        tr.appendChild(td5);
        table.appendChild(tr);
    }
    tableDiv.appendChild(table);
    var tableIdentify = document.querySelector('.lexem-table-identity');
    var identityTable = document.createElement('table');
    var theadIdentity = document.createElement('thead');
    var th1Identity = document.createElement('th');
    th1Identity.innerText = "ID";
    var th2Identity = document.createElement('th');
    th2Identity.innerText = "Token";
    var th3Identity = document.createElement('th');
    th3Identity.innerText = "Type"
    theadIdentity.appendChild(th1Identity);
    theadIdentity.appendChild(th2Identity);
    theadIdentity.appendChild(th3Identity);
    identityTable.appendChild(theadIdentity);
    for (var i = 0; i < result.tables._IdentifyTable.length; i++){
        var tr = document.createElement('tr');
        var td1 = document.createElement('td');
        td1.innerText = (i+1).toString();
        var td2 = document.createElement('td');
        td2.innerText = result.tables._IdentifyTable[i].Token;
        var td3 = document.createElement('td');
        td3.innerText = result.tables._IdentifyTable[i].Type;
        tr.appendChild(td1);
        tr.appendChild(td2);
        tr.appendChild(td3);
        identityTable.appendChild(tr);
    }
    tableIdentify.appendChild(identityTable);

    var tableConst = document.querySelector('.lexem-table-const');
    var constTable = document.createElement('table');
    var theadConst = document.createElement('thead');
    var th1Const = document.createElement('th');
    th1Const.innerText = "ID";
    var th2Const = document.createElement('th');
    th2Const.innerText = "Token";
    theadConst.appendChild(th1Const);
    theadConst.appendChild(th2Const);
    constTable.appendChild(theadConst);

    for (var i = 0; i < result.tables._ConstTable.length; i++) {
        var tr = document.createElement('tr');
        var td1 = document.createElement('td');
        var td2 = document.createElement('td');
        td1.innerText = (i + 1).toString();
        td2.innerText = result.tables._ConstTable[i].Token;
        tr.appendChild(td1);
        tr.appendChild(td2);
        constTable.appendChild(tr);
    }
    tableConst.appendChild(constTable);
    

    $.ajax({
        type: 'POST',
        url: "../Home/SyntacticAnalyzer/",
        async: false,
        data: {
            
        },
        beforeSend:function(){
            var loader = document.querySelector('#loader');
            loader.style.display = 'block';
        },
        success: onSyntacticAnalyzeSuccess
    });
}

function onSyntacticAnalyzeSuccess(result) {
    var loader = document.querySelector('#loader');
    loader.style.display = 'none';
    var console = document.querySelector('#console');
    console.value += "Syntactic analyzer: " + result.parserResult + '\n';

    var tableDiv = document.querySelector('.ascending-parser-table');

    var table = document.createElement('table');

    for (var i = 0; i < result.parserTable.length; i++)
    {
        var tr = document.createElement('tr');
        var td1 = document.createElement('td');
        var td2 = document.createElement('td');
        var td3 = document.createElement('td');

        td1.innerText = '...' + result.parserTable[i].First10Stack;
        td2.innerText = result.parserTable[i].Relation;
        td3.innerText = result.parserTable[i].LexemString;
        tr.appendChild(td1);
        tr.appendChild(td2);
        tr.appendChild(td3);
        table.appendChild(tr);
    }
    tableDiv.appendChild(table);

    $.ajax({
        type: 'POST',
        url: "../Home/PolizBilder/",
        async: false,
        data: {

        },
        beforeSend:function(){
            var loader = document.querySelector('#loader');
            loader.style.display = 'block';
        },
        success: onPolizBuildSuccess
    });
}

function onPolizBuildSuccess(result){
    var console = document.querySelector('#console');
    console.value += "Poliz: " + result.polizStr + '\n';
    var loader = document.querySelector('#loader');
    loader.style.display = 'none';
    $.ajax({
        type: 'Post',
        url: "../Home/PolizExecute/",
        async: false,
        data: {
            startParam: 1
        },
        beforeSend:function(){
            var loader = document.querySelector('#loader');
            loader.style.display = 'block';
        },
        success: onStepSuccess
    });
}

function onStepSuccess(result) {
    var loader = document.querySelector('#loader');
    loader.style.display = 'none';
    if (result.Status == 'Successful Done') {
        alert('Successful Done');
        return;
    }
    else if (result.Status.indexOf('initialized') != -1) {
        alert(result.Status);
        return;
    }
    var inputParam;
    if (result.Status == 'Input int') {
        inputParam = prompt('input int32 value please: ', 0);
        if (!(inputParam === null)) {
            inputParam = parseInt(inputParam);
            if (isNaN(inputParam)) {
                alert('invalid value was input');
                onAjaxExecuteSuccess(result);
                return;
            }

        }
    }
    else if (result.Status == 'Input double') {
        inputParam = prompt('input double value please: ', 0);
        if (!(inputParam === null)) {
            inputParam = parseFloat(inputParam);
            if (isNaN(inputParam)) {
                alert('invalid value was input');
                onAjaxExecuteSuccess(result);
                return;
            }
        }
    }
    else if (result.Status == 'Output int' || result.Status == 'Output double') {
        //alert(result.OutParam);
        var console = document.querySelector('#console');
        console.value += '\n';
        console.value += result.IdnName + ' = ' + result.OutParam;
        $.ajax({
            type: "POST",
            url:"../Home/OutputParam/",
            data:{

            },
            success: onStepSuccess
        });
        return;
    }
    $.ajax({
        type: 'POST',
        url: '../Home/InputParam/',
        data: {
            param: inputParam
        },
        success: onStepSuccess,
    })
}