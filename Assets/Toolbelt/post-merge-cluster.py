import os
import sys

import shutil

#run this once after your first pull.
#after that, it should call itself automatically!

def smartCopy(src, dst):
    if not os.path.isfile(dst):
        print 'copying from:' + src + ' to:' + dst
        shutil.copy2(src,dst)
        return
    if os.stat(src).st_mtime - os.stat(dst).st_mtime > 1:
        print 'updating from:' + src + ' to:' + dst
        shutil.copy2 (src, dst)    
    else:
        print 'file up to date:' + src + ' to:' + dst
        
def copyPlugin(name):
    src = 'Plugins/'
    dst = '../Plugins/'
    
    smartCopy(src+name, dst+name)
    
#post-merge script copy
dst = None
if (os.path.isdir('.git')):
    dst = '.git/hooks/post-merge'
else:
    with open('.git','r') as f:
        for line in f:
            if line.startswith('gitdir'):
                dst = line[8:].strip() + '/hooks/post-merge'
                break
    print dst   
if dst is not None:
    smartCopy('post-merge-cluster',dst);

try:
    os.stat('../Plugins')
except:
    os.mkdir('../Plugins')

copyPlugin('NatNetClientInterface.dll')
copyPlugin('bink_unity_plugin.dll')
copyPlugin('Ionic.Zip.dll')
smartCopy('Plugins/binkw32.dll','../../binkw32.dll')
smartCopy('Plugins/smcs-cluster.rsp','../smcs.rsp')
smartCopy('Plugins/NatNetLib.dll','../../NatNetLib.dll')
