# The board is 7x7
class Field(object):
    def __init__(self, top=False, bottom=False, left=False, right=False, players=None, treasure=""):
        self.top = top
        self.bottom = bottom
        self.left = left
        self.right = right
        self.players = players
        self.treasure = treasure
        object.__init__(self)

    def __str__(self):
        asciiart = "\n"
        if self.top:
            asciiart+="# #\n"
        else:
            asciiart+="###\n"

        if self.left:
            asciiart+="  "
        else:
            asciiart+="# "

        if self.right:
            asciiart+="\n"
        else:
            asciiart+="#\n"

        if self.bottom:
            asciiart+="# #"
        else:
            asciiart+="###"

        text = "Top: {}, Bottom: {}, Left: {}, Right: {}, Players: {}, Treasure: {}"
        string = text.format(self.top, self.bottom,self.left, self.right,self.players,self.treasure)
        return asciiart + "\n" + string


def Initialize(board, verbose=False):
    print("Initializing with board")
    PrintBoard(board)



def PrintBoard(board):
    for i in range(0, 7):
        for j in range(0, 7):
            print("{}:{} - {}".format(i, j, board[i][j]))

def ShiftBoard(board,index,field):
    """index defines the (x,y) position from where the board is shifted inwards"""
    print("Shifting {} into board at position {}",field,index)
    if index.x == 0:
        print("shifting downward")
        line = DownwardList(board,index.y)
    elif index.y == 0:
        print("shifting rightward")
        line= RightwardList(board,index.x)
    elif index.x == 6:
        print("shifting leftward")
        line=LeftwardList(board,index.y)
    elif index.y == 6:
        print("shifting upward")
        line=UpwardList(board,index.x)
    else:
        print("ERROR ON SHIFTING INDEX")

    print("Line is ")

def DownwardList(board,start):
    list = []
    for i in range(0,7):
        list.append(board[start][i])
    return list
def RightwardList(board,start):
    pass
def LeftwardList(board,start):
    pass
def UpwardList(board,start):
    pass