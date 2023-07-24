// get the args from the command line
const args = process.argv.slice(2);
let fs = require('fs');
if (args.length !== 1) {
    console.error('Usage: ts-node generate_ast.ts <output directory>');
}

type AstType = {
    name: string;
    fields: string[];
};

type Ast = {
    baseName: string;
    children: AstType[];
};

const outputDir = args[0];

const expressions: Ast = {
    baseName: 'Expr',
    children: [
        { name: 'Assign', fields: ['Token name', 'Expr value'] },
        { name: 'Binary', fields: ['Expr left', 'Token op', 'Expr right'] },
        { name: 'Grouping', fields: ['Expr expr'] },
        { name: 'Literal', fields: ['object value'] },
        { name: 'Unary', fields: ['Token op', 'Expr right'] },
        { name: 'Var', fields: ['Token name'] },
    ],
};

const statements: Ast = {
    baseName: 'Stmt',
    children: [
        { name: 'Block', fields: ['List<Stmt> statements'] },
        { name: 'Expression', fields: ['Expr expr'] },
        { name: 'Print', fields: ['Expr expr'] },
        { name: 'Var', fields: ['Token name', 'Expr initializer'] },
    ]
};

defineAst(outputDir, expressions);
defineVisitorInterface(outputDir, expressions);

defineAst(outputDir, statements);
defineVisitorInterface(outputDir, statements);

function defineAst(outputDir: string, ast: Ast) {
    let output = `namespace cshlox;` + '\n\n';
    output += `public abstract class ${ast.baseName}\n{` + '\n';
    output += `\tpublic abstract T Accept<T>(I${ast.baseName}Visitor<T> visitor);` + '\n';

    ast.children.forEach((type) => {
        output += '\n';
        output += `\tpublic class ${type.name} : ${ast.baseName}\n\t{` + '\n';
        type.fields.forEach((field) => {
            let fields = field.split(' ');
            let fieldName = fields[1].charAt(0).toUpperCase() + fields[1].slice(1);
            output += `\t\tpublic ${fields[0]} ${fieldName} { get; }` + '\n';
        });

        output += '\n';
        output += `\t\tpublic ${type.name}(${type.fields.join(', ')})\n\t\t{` + '\n';
        type.fields.forEach((field) => {
            let fields = field.split(' ');
            let fieldName = fields[1].charAt(0).toUpperCase() + fields[1].slice(1);
            output += `\t\t\t${fieldName} = ${fields[1]};` + '\n';
        });

        output += '\t\t}' + '\n\n';
        output += defineVisitor(ast.baseName, type.name);
        output += '\t}' + '\n';
    });

    output += '}' + '\n';

    if (!fs.existsSync(outputDir)) {
        fs.mkdirSync(outputDir);
    }

    fs.writeFileSync(`${outputDir}/${ast.baseName}.cs`, output);
}

function defineVisitorInterface(outputDir: string, ast: Ast) {
    let output = `namespace cshlox;` + '\n\n';

    output += `public interface I${ast.baseName}Visitor<T>` + '\n';
    output += '{' + '\n';

    ast.children.forEach((type) => {
        // implement this code below but using string interpolation
        output += `\tT Visit${type.name}${ast.baseName}(${ast.baseName}.${type.name} ${ast.baseName.toLowerCase()});` + '\n';
    });

    output += '}' + '\n';

    fs.writeFileSync(outputDir + `/I${ast.baseName}Visitor.cs`, output);
}

function defineVisitor(baseName: string, typeName: string) {
    let output = `\t\tpublic override T Accept<T>(I${baseName}Visitor<T> visitor)\n\t\t{` + '\n';

    output += '\t\t\treturn visitor.Visit' + typeName + baseName + '(this);' + '\n';
    output += '\t\t}' + '\n';

    return output;
}


