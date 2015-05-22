from random import getrandbits
from MonteCarlPY import Field
import MonteCarlPY as mcpy

#initialize random board
#0,0 is top left (x,y)
board = []
print("initializing board")
for i in range (0,7):
    board.append([])
    for j in range (0,7):
        board[i].append(Field(bool(getrandbits(1)),bool(getrandbits(1)),bool(getrandbits(1)),bool(getrandbits(1)),[],""))
print("completed")

#define start positions
board[0][0] = Field(False,True,False,True,[1],"Start01")
board[6][0] = Field(False,True,True,False,[2],"Start02")
board[0][6] = Field(True,False,False,True,[3],"Start03")
board[6][6] = Field(True,False,True,False,[4],"Start04")

#test
print("initializing algorithm")
mcpy.Initialize(board=board,verbose=True)