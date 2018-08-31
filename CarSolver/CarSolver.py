import copy
import sys
import faulthandler
faulthandler.enable()

#import objgraph
sys.setrecursionlimit(10000) 

class Point:
    def __init__(self,X,Y):
        #__slots__ = {'x','y'}
        self.x = X
        self.y = Y

    def __repr__(self):
        return "(" + str(self.x)+ "," + str(self.y) + ")"
        
class Car:
    def __init__(self,pos,dir,size,name):
        self.pos = pos
        self.dir = dir  #H or V
        self.size = size
        self.name = name
    def __repr__(self):
        return "Car name=" + self.name + " pos=" + str(self.pos) + " dir=" + self.dir + " size=" + str(self.size)

class Board:
    def __init__(self,w,h):
        self.out = Point(w-1,int(h/2))
        self.w = w
        self.h = h
       # self.table = [["_" for x in range(w)] for y in range(h)] 
        self.cars = []
        self.moves = []

    def __repr__(self):
        table = self.render()
        #table[self.out.y][self.out.x] = 'X'   

        ret = ""
        for row in table:
            for col in row:
                ret += col + " "
            ret += "\n"
        return ret

    def addCar(self,x,y,car):
        carNew = copy.deepcopy(car)
        carNew.pos = Point(x,y)
        self.cars.append(carNew)

    def render(self):
        table = [["_" for x in range(self.w)] for y in range(self.h)] 
        for car in self.cars:
            x = car.pos.x
            y = car.pos.y
            for i in range(0,car.size):
                if car.dir == "H":
                    table[y][x+i] = car.name
                else:
                    table[y+i][x] = car.name
        return table

    def isFinish(self):
        carP = self.cars[0]
        return carP.pos.x+1 == self.out.x  and  carP.pos.y == self.out.y 

    def isTheSame(self,board):
        return str(self) == str(board)


    def GetAllPossibleMove(self):
        
        table = self.render()
        move = []
        for car in self.cars:
            x = car.pos.x
            y = car.pos.y

            if car.dir == "H":
                if x+car.size < self.w and table[y][x+car.size] == '_' :
                    move.append(Move(car,1,0))
                if x > 0 and table[y][x-1] == '_':
                    move.append(Move(car,-1,0))
                  
            else:
                if y+car.size < self.h and table[y+car.size][x] == '_':
                    move.append(Move(car,0,1))
                if y > 0 and table[y-1][x] == '_':
                    move.append(Move(car,0,-1))
        

        return move
        pass
    def move(self,move):
        for car in self.cars:
            if car.name == move.car.name:
                car.pos.x += move.dx 
                car.pos.y += move.dy
                self.moves.append(move)
                return           

class Move:
    def __init__(self,car,dx,dy):
        self.car = copy.deepcopy(car)
        self.dx = dx    
        self.dy = dy

    def __repr__(self):
        return "Move name=" + self.car.name + " " + str(self.dx) + " " + str(self.dy)

def solve2(board):
    seenBoard = []
    workingBoard = []
    workingBoard.append(board)
    while len(workingBoard) > 0:
        print(len(workingBoard),len(seenBoard))    
        wBoard = workingBoard.pop(0)
        if wBoard.isFinish():
            print("FOUND")
            print(wBoard)
            return
        strwBoard = str(wBoard)      
        try:
            seenBoard.index(strwBoard)
            continue
        except:
            pass
        print(strwBoard)                    
        print(wBoard.moves)
        seenBoard.append(strwBoard)      
        moves = wBoard.GetAllPossibleMove()
        for move in moves:
            newBoard = copy.deepcopy(wBoard)
            newBoard.move(move)
            workingBoard.append(newBoard)
    print("NOT FOUND")
    
def solve(board):
    global seenBoard
    global x
    if board.isFinish() :
        print("FOUND")
        return True
    seenBoard.append(str(board))

    if x > 39000:
        return False
    x += 1
    print(x)
    print(board)
    moves = board.GetAllPossibleMove();
    print(moves)
    for move in moves:
        newBoard = copy.deepcopy(board)
        newBoard.move(move)
        #print(newBoard)
        #Check if this board is finish
        found = False
        strNewBoard = str(newBoard)
        for b in seenBoard:
            if strNewBoard == b:
                found = True
                break
        if found == False:
            if solve(newBoard):
                return True
    return False            

def findMostMove(board):
    bestBoard = board
    seenBoard = []
    workingBoard = []
    workingBoard.append(board)
    while len(workingBoard) > 0:
        print(len(workingBoard),len(seenBoard))    
        wBoard = workingBoard.pop(0)
        strwBoard = str(wBoard)      
        try:
            seenBoard.index(strwBoard)
            continue
        except:
            pass
        print(strwBoard,len(wBoard.moves))
        seenBoard.append(strwBoard)      
        if len(wBoard.moves) > len(bestBoard.moves):
            bestBoard = wBoard
        moves = wBoard.GetAllPossibleMove()
        for move in moves:
            newBoard = copy.deepcopy(wBoard)
            newBoard.move(move)
            workingBoard.append(newBoard)
    return  bestBoard
    
seenBoard = []
board = Board(7,6)
board.out = Point(6,3)
#board.addCar(3,3,Car(Point(0,0),"H",2,"P"))
#board.addCar(0,0,Car(Point(0,0),"V",3,"S"))
#board.addCar(1,0,Car(Point(0,0),"H",2,"D"))
#board.addCar(1,1,Car(Point(0,0),"H",2,"G"))
#board.addCar(2,2,Car(Point(0,0),"V",2,"C"))
#board.addCar(3,0,Car(Point(0,0),"V",2,"K"))
#board.addCar(4,0,Car(Point(0,0),"V",2,"B"))
#board.addCar(5,0,Car(Point(0,0),"H",2,"J"))
#board.addCar(5,1,Car(Point(0,0),"H",2,"H"))
#board.addCar(3,2,Car(Point(0,0),"H",3,"Q"))
#board.addCar(6,2,Car(Point(0,0),"V",2,"L"))
#board.addCar(5,3,Car(Point(0,0),"V",3,"A"))
#board.addCar(3,4,Car(Point(0,0),"H",2,"N"))
#board.addCar(6,4,Car(Point(0,0),"V",2,"F"))
#board.addCar(2,5,Car(Point(0,0),"H",3,"O"))

board.addCar(4,3,Car(Point(0,0),"H",2,"P"))
board.addCar(0,2,Car(Point(0,0),"V",3,"S"))
board.addCar(0,0,Car(Point(0,0),"H",2,"D"))
board.addCar(0,1,Car(Point(0,0),"H",2,"G"))
board.addCar(2,0,Car(Point(0,0),"V",2,"C"))
board.addCar(3,4,Car(Point(0,0),"V",2,"K"))
board.addCar(4,4,Car(Point(0,0),"V",2,"B"))
board.addCar(3,0,Car(Point(0,0),"H",2,"J"))
board.addCar(3,1,Car(Point(0,0),"H",2,"H"))
board.addCar(1,2,Car(Point(0,0),"H",3,"Q"))
board.addCar(6,1,Car(Point(0,0),"V",2,"L"))
board.addCar(5,0,Car(Point(0,0),"V",3,"A"))
board.addCar(1,4,Car(Point(0,0),"H",2,"N"))
board.addCar(6,4,Car(Point(0,0),"V",2,"F"))
board.addCar(0,5,Car(Point(0,0),"H",3,"O"))


x=1
try:
    #solve2(board)
    bestBoard = findMostMove(board)
    print(bestBoard,len(bestBoard.moves))
except Exception as e:
    print(e)

